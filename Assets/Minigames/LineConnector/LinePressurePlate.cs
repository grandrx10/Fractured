using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LinePressurePlate : MonoBehaviour
{
    public Vector2Int gridPos;

    [Header("Dot Settings")]
    public bool isDot;
    public int pairId = -1;

    [Header("Runtime State")]
    [HideInInspector] public bool occupied;
    [HideInInspector] public int occupiedByPair = -1;

    [Header("Visuals")]
    [SerializeField] private Renderer plateRenderer;

    [Header("Transition Settings")]
    [SerializeField] private float colorLerpDuration = 0.15f;
    [SerializeField] private float pieceTransitionDuration = 0.2f;

    private GridPuzzleManager manager;
    private Material materialInstance;
    private Color dotColor;
    private Rigidbody rb;

    private Coroutine colorRoutine;
    private Coroutine pieceRoutine;

    // Connection tracking (N, E, S, W)
    private bool[] connections = new bool[4];
    private int currentPieceType = 0;
    private float currentRotation = 0f;
    
    /* PIECE TYPE MAPPING:
     * 0 = no connections (empty)
     * 1 = one connection (pointing North at 0°)
     * 2 = two connections straight (North-South at 0°, East-West at 90°)
     * 3 = two connections curved (North-West corner at 0°, then rotates counter-clockwise)
     * 4 = dot with no connections
     * 5 = dot with one connection (pointing North at 0°)
     * 
     * ROTATION: 0° = North, 90° = East, 180° = South, 270° = West
     */

    public void Init(GridPuzzleManager mgr, Vector2Int pos)
    {
        manager = mgr;
        gridPos = pos;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        if (plateRenderer == null)
            plateRenderer = GetComponentInChildren<Renderer>();

        materialInstance = new Material(plateRenderer.material);
        plateRenderer.material = materialInstance;

        ResetVisual();
    }

    // ---------------- PHYSICS ----------------

    public void ReleaseAndDestroy(float delay)
    {
        if (rb != null)
            rb.isKinematic = false;

        Destroy(gameObject, delay);
    }

    // ---------------- STATE ----------------

    public void SetOccupied(int pair)
    {
        occupied = true;
        occupiedByPair = pair;

        TransitionToColor(manager.GetColorForPair(pair));
        UpdateConnections();
    }

    public void SetDotColor(Color color)
    {
        dotColor = color;
        materialInstance.color = color;
        materialInstance.SetFloat("_isDot", 1f);
        materialInstance.SetFloat("_piece", 4f); // dot_0
        materialInstance.SetFloat("_oldPiece", 4f);
    }

    public void Clear()
    {
        occupied = false;
        occupiedByPair = -1;

        Color target = isDot ? dotColor : Color.gray;
        TransitionToColor(target);
        
        // Reset connections
        connections = new bool[4];
        UpdatePieceVisual();
    }

    void ResetVisual()
    {
        materialInstance.color = Color.gray;
        materialInstance.SetFloat("_piece", 0f);
        materialInstance.SetFloat("_oldPiece", 0f);
        materialInstance.SetFloat("_rotation", 0f);
        materialInstance.SetFloat("_oldRotation", 0f);
        materialInstance.SetFloat("_isDot", 0f);
        materialInstance.SetFloat("_t", 1f);
    }

    // ---------------- CONNECTIONS ----------------

    public void UpdateConnections()
    {
        // Get neighbors from manager
        var neighbors = manager.GetNeighbors(this);
        
        // North, East, South, West
        connections[0] = ShouldConnectTo(neighbors[0]);
        connections[1] = ShouldConnectTo(neighbors[1]);
        connections[2] = ShouldConnectTo(neighbors[2]);
        connections[3] = ShouldConnectTo(neighbors[3]);

        UpdatePieceVisual();
    }

    bool ShouldConnectTo(LinePressurePlate neighbor)
    {
        if (neighbor == null) return false;
        if (!neighbor.occupied) return false;
        if (neighbor.occupiedByPair != occupiedByPair) return false;
        
        // Must be adjacent in the path sequence (within 1 step)
        int myIndex = manager.GetPlateIndexInPath(this, occupiedByPair);
        int neighborIndex = manager.GetPlateIndexInPath(neighbor, occupiedByPair);
        
        if (myIndex == -1 || neighborIndex == -1) return false;
        
        // Only connect if neighbor is exactly +1 or -1 in the path
        return Mathf.Abs(myIndex - neighborIndex) == 1;
    }

    void UpdatePieceVisual()
    {
        int connectionCount = 0;
        foreach (bool conn in connections)
            if (conn) connectionCount++;

        int oldPiece = currentPieceType;
        float oldRotation = currentRotation;
        int newPiece = 0;
        float rotation = 0f;

        if (isDot)
        {
            // Dots: piece 4 (no connections) or 5 (1 connection)
            if (connectionCount == 0)
            {
                newPiece = 4; // dot_0
            }
            else if (connectionCount == 1)
            {
                newPiece = 5; // dot_1
                // Rotate to match the single connection
                rotation = GetRotationForSingleConnection();
            }
        }
        else
        {
            // Regular pieces
            switch (connectionCount)
            {
                case 0:
                    newPiece = 0; // no connections
                    break;
                case 1:
                    newPiece = 1; // 1 connection
                    rotation = GetRotationForSingleConnection();
                    break;
                case 2:
                    if (IsStraightConnection())
                    {
                        newPiece = 2; // straight
                        rotation = connections[0] && connections[2] ? 0f : 90f; // N-S = 0°, E-W = 90°
                    }
                    else
                    {
                        newPiece = 3; // curved
                        rotation = GetRotationForCurvedConnection();
                    }
                    break;
                case 3:
                    newPiece = 1; // Use 1-connection piece, rotate to missing direction
                    rotation = GetRotationForThreeConnections();
                    break;
                case 4:
                    newPiece = 2; // Use straight piece for cross
                    break;
            }
        }

        currentPieceType = newPiece;
        currentRotation = rotation;

        TransitionToPiece(oldPiece, newPiece, oldRotation, rotation);
    }

    // ---------------- CONNECTION HELPERS ----------------

    bool IsStraightConnection()
    {
        return (connections[0] && connections[2]) || (connections[1] && connections[3]);
    }

    float GetRotationForSingleConnection()
    {
        if (connections[0]) return 180f;   // North
        if (connections[1]) return 90f;  // East
        if (connections[2]) return 0f; // South
        if (connections[3]) return 270f; // West
        return 0f;
    }

    float GetRotationForCurvedConnection()
    {
        // N-W corner (counter-clockwise from here)
        if (connections[0] && connections[3]) return 180f;
        // N-E corner
        if (connections[0] && connections[1]) return 90f;
        // S-E corner
        if (connections[2] && connections[1]) return 0f;
        // S-W corner
        if (connections[2] && connections[3]) return 270f;
        return 0f;
    }

    float GetRotationForThreeConnections()
    {
        // Point toward the missing direction (opposite of 1-connection logic)
        if (!connections[0]) return 180f; // Missing North, point South
        if (!connections[1]) return 270f; // Missing East, point West
        if (!connections[2]) return 0f;   // Missing South, point North
        if (!connections[3]) return 90f;  // Missing West, point East
        return 0f;
    }

    // ---------------- COLOR LERP ----------------

    void TransitionToColor(Color target)
    {
        if (colorRoutine != null)
            StopCoroutine(colorRoutine);

        colorRoutine = StartCoroutine(LerpColor(target));
    }

    IEnumerator LerpColor(Color target)
    {
        Color start = materialInstance.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / colorLerpDuration;
            materialInstance.color = Color.Lerp(start, target, t);
            yield return null;
        }

        materialInstance.color = target;
        colorRoutine = null;
    }

    // ---------------- PIECE TRANSITION ----------------

    void TransitionToPiece(int oldPiece, int newPiece, float oldRotation, float newRotation)
    {
        if (pieceRoutine != null)
            StopCoroutine(pieceRoutine);

        pieceRoutine = StartCoroutine(LerpPiece(oldPiece, newPiece, oldRotation, newRotation));
    }

    IEnumerator LerpPiece(int oldPiece, int newPiece, float oldRotation, float newRotation)
    {
        materialInstance.SetFloat("_oldPiece", oldPiece);
        materialInstance.SetFloat("_piece", newPiece);
        materialInstance.SetFloat("_oldRotation", oldRotation);
        materialInstance.SetFloat("_rotation", newRotation);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / pieceTransitionDuration;
            materialInstance.SetFloat("_t", t);
            yield return null;
        }

        materialInstance.SetFloat("_t", 1f);
        materialInstance.SetFloat("_oldPiece", newPiece);
        materialInstance.SetFloat("_oldRotation", newRotation);
        pieceRoutine = null;
    }

    // ---------------- INPUT ----------------

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        manager.OnPlateStepped(this);
    }

    private void OnDestroy()
    {
        if (materialInstance != null)
            Destroy(materialInstance);
    }
}