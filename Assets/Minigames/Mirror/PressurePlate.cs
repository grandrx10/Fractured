using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The trigger collider to monitor for collisions")]
    public Collider triggerCollider;

    [Tooltip("The GameObject that will move down when pressed")]
    public GameObject pressurePlate;

    [Header("Plate Settings")]
    [Tooltip("How far the plate compresses down when pressed")]
    public float compressionDistance = 0.1f;

    [Tooltip("Speed of the compression animation")]
    public float compressionSpeed = 5f;

    [Tooltip("If true, plate stays pressed after first activation")]
    public bool stayPressed = false;

    [Header("Events")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;
    private bool hasBeenPressed = false;
    private int objectsOnPlate = 0;

    private void Start()
    {
        if (triggerCollider == null)
        {
            Debug.LogError("PressurePlate: triggerCollider is not assigned!");
            return;
        }

        if (pressurePlate == null)
        {
            Debug.LogError("PressurePlate: pressurePlate GameObject is not assigned!");
            return;
        }

        if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning("PressurePlate: Assigned collider is not set as trigger!");
        }

        originalPosition = pressurePlate.transform.localPosition;
        pressedPosition = originalPosition + Vector3.down * compressionDistance;

        // Subscribe to the trigger events
        TriggerEventForwarder forwarder = triggerCollider.gameObject.GetComponent<TriggerEventForwarder>();
        if (forwarder == null)
        {
            forwarder = triggerCollider.gameObject.AddComponent<TriggerEventForwarder>();
        }
        forwarder.SetTarget(this);
    }

    private void Update()
    {
        if (pressurePlate == null) return;

        // Check for overlapping objects (handles teleportation cases)
        CheckForOverlaps();

        // Determine target position based on state
        Vector3 targetPosition = (isPressed || (stayPressed && hasBeenPressed)) 
            ? pressedPosition 
            : originalPosition;

        // Smoothly move to target position
        pressurePlate.transform.localPosition = Vector3.Lerp(
            pressurePlate.transform.localPosition,
            targetPosition,
            Time.deltaTime * compressionSpeed
        );
    }

    private void CheckForOverlaps()
    {
        if (triggerCollider == null) return;

        // Get all overlapping colliders
        Collider[] overlaps = Physics.OverlapBox(
            triggerCollider.bounds.center,
            triggerCollider.bounds.extents,
            triggerCollider.transform.rotation
        );

        // Count valid overlaps (excluding the trigger itself)
        int currentOverlaps = 0;
        foreach (Collider col in overlaps)
        {
            if (col != triggerCollider)
            {
                currentOverlaps++;
            }
        }

        // If something is overlapping and we're not pressed
        if (currentOverlaps > 0 && !isPressed)
        {
            isPressed = true;
            hasBeenPressed = true;
            objectsOnPlate = currentOverlaps;
            onPressed?.Invoke();
            Debug.Log($"Pressure plate pressed (overlap detected)");
        }
        // If nothing is overlapping and we're pressed (and not stayPressed)
        else if (currentOverlaps == 0 && isPressed && !stayPressed)
        {
            isPressed = false;
            objectsOnPlate = 0;
            onReleased?.Invoke();
            Debug.Log("Pressure plate released (no overlaps)");
        }
    }

    public void OnTriggerEnterReceived(Collider other)
    {
        objectsOnPlate++;

        if (!isPressed)
        {
            isPressed = true;
            hasBeenPressed = true;
            onPressed?.Invoke();
            Debug.Log($"Pressure plate pressed by: {other.name}");
        }
    }

    public void OnTriggerExitReceived(Collider other)
    {
        objectsOnPlate--;

        if (objectsOnPlate <= 0 && !stayPressed)
        {
            objectsOnPlate = 0;
            isPressed = false;
            onReleased?.Invoke();
            Debug.Log("Pressure plate released");
        }
    }

    public void ResetPlate()
    {
        isPressed = false;
        hasBeenPressed = false;
        objectsOnPlate = 0;
        onReleased?.Invoke();
    }

    public bool IsPressed()
    {
        return isPressed || (stayPressed && hasBeenPressed);
    }
}

// Helper component to forward trigger events from the collider to the PressurePlate
public class TriggerEventForwarder : MonoBehaviour
{
    private PressurePlate target;

    public void SetTarget(PressurePlate plate)
    {
        target = plate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target != null)
            target.OnTriggerEnterReceived(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (target != null)
            target.OnTriggerExitReceived(other);
    }
}