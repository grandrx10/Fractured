using UnityEngine;

public class ScaleTilt : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target;

    [Header("Tilt Settings")]
    public float tiltMultiplier = 30f; // How much tilt per unit of vertical movement
    public float maxTilt = 30f;        // Maximum tilt angle in degrees
    public float smoothSpeed = 5f;     // How fast the scale tilts

    private Vector3 lastTargetPos;
    private Quaternion initialRotation;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("ScaleTilt: No target assigned.");
            enabled = false;
            return;
        }

        lastTargetPos = target.position;
        initialRotation = transform.localRotation; // Store the initial rotation
    }

    private void Update()
    {
        if (target == null) return;

        // Calculate vertical movement delta (Y-axis)
        Vector3 delta = target.position - lastTargetPos;

        // Tilt based on vertical movement
        // Positive Y movement (going up) tilts backward (positive X rotation)
        // Negative Y movement (going down) tilts forward (negative X rotation)
        float desiredTiltX = delta.y * tiltMultiplier;
        
        // Tilt based on Z movement (forward/back)
        float desiredTiltZ = -delta.z * tiltMultiplier;

        // Clamp the tilt to max angle
        desiredTiltX = Mathf.Clamp(desiredTiltX, -maxTilt, maxTilt);
        desiredTiltZ = Mathf.Clamp(desiredTiltZ, -maxTilt, maxTilt);

        // Create tilt rotation relative to initial rotation
        Quaternion tiltRotation = Quaternion.Euler(desiredTiltX, 0f, desiredTiltZ);
        Quaternion targetRotation = initialRotation * tiltRotation;

        // Smoothly interpolate to target rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothSpeed);

        // Update last position
        lastTargetPos = target.position;
    }
}