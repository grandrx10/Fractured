using UnityEngine;

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
    [Tooltip("Renderer on the child object that represents the plate visually")]
    [SerializeField] private Renderer plateRenderer;

    private GridPuzzleManager manager;
    private Material materialInstance; // Store material instance
    private Color dotColor; // Store the dot's original color

    public void Init(GridPuzzleManager mgr, Vector2Int pos)
    {
        manager = mgr;
        gridPos = pos;

        // Auto-find renderer if not assigned
        if (plateRenderer == null)
            plateRenderer = GetComponentInChildren<Renderer>();

        if (plateRenderer == null)
        {
            Debug.LogError($"LinePressurePlate at {gridPos} has no Renderer assigned or found in children.");
            return;
        }

        // Create a material instance to avoid modifying the shared material
        materialInstance = new Material(plateRenderer.material);
        plateRenderer.material = materialInstance;

        ResetVisual();
    }

    // ---------------- STATE ----------------

    public void SetOccupied(int pair)
    {
        occupied = true;
        occupiedByPair = pair;
        
        if (materialInstance != null)
            materialInstance.color = manager.GetColorForPair(pair);
    }

    // New method specifically for setting dot colors without marking as occupied
    public void SetDotColor(Color color)
    {
        dotColor = color;
        if (materialInstance != null)
            materialInstance.color = color;
    }

    public void Clear()
    {
        occupied = false;
        occupiedByPair = -1;
        
        // If this is a dot, restore its original color instead of gray
        if (isDot)
        {
            if (materialInstance != null)
                materialInstance.color = dotColor;
        }
        else
        {
            ResetVisual();
        }
    }

    // ---------------- VISUALS ----------------

    void ResetVisual()
    {
        if (materialInstance != null)
            materialInstance.color = Color.gray;
    }

    // ---------------- INPUT ----------------

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        manager.OnPlateStepped(this);
    }

    // Clean up material instance
    private void OnDestroy()
    {
        if (materialInstance != null)
            Destroy(materialInstance);
    }
}