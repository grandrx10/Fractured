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

    [Header("Color Transition")]
    [SerializeField] private float colorLerpDuration = 0.15f;

    private GridPuzzleManager manager;
    private Material materialInstance;
    private Color dotColor;
    private Rigidbody rb;

    private Coroutine colorRoutine;

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

        Color target = isDot ? dotColor : Color.gray;
        TransitionToColor(target);
    }

    void ResetVisual()
    {
        materialInstance.color = Color.gray;
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
