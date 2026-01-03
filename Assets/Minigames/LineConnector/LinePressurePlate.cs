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

    private GridPuzzleManager manager;
    private Material materialInstance;
    private Color dotColor;
    private Rigidbody rb;

    public void Init(GridPuzzleManager mgr, Vector2Int pos)
    {
        manager = mgr;
        gridPos = pos;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;     // IMPORTANT: start kinematic
        // rb.useGravity = true;

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
        materialInstance.color = manager.GetColorForPair(pair);
    }

    public void SetDotColor(Color color)
    {
        dotColor = color;
        materialInstance.color = color;
    }

    public void Clear()
    {
        occupied = false;
        occupiedByPair = -1;

        materialInstance.color = isDot ? dotColor : Color.gray;
    }

    void ResetVisual()
    {
        materialInstance.color = Color.gray;
    }

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
