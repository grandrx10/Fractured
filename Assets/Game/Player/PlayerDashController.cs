using UnityEngine;
using System.Collections;
using Characters;

[RequireComponent(typeof(Rigidbody))]
public class PlayerDashController : MonoBehaviour
{
    private bool isDashing = false;
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private Vector3 dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    /// <summary>
    /// Gets the movement direction from WASD input on the XZ plane,
    /// relative to the camera's forward direction from PlayerInteractController
    /// </summary>
    private Vector3 GetInputDirection()
    {
        // Use main camera if raycastOrigin is not accessible
        Transform cameraTransform = Camera.main.transform;

        // Camera forward/right
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Flatten to XZ plane
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;
        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;

        Vector3 direction = cameraForward * vertical + cameraRight * horizontal;

        // Default to forward if no input
        if (direction == Vector3.zero)
            direction = cameraForward; // W key forward

        return direction.normalized;
    }


    /// <summary>
    /// Start a dash in the current movement direction based on keyboard input.
    /// </summary>
    public void StartDash(float dashSpeed, float dashDuration)
    {
        if (isDashing) return;
        
        Vector3 inputDirection = GetInputDirection();
        
        // Only dash if there's input
        if (inputDirection != Vector3.zero)
        {
            dashDirection = inputDirection;
            StartCoroutine(DashCoroutine(dashSpeed, dashDuration));
        }
    }

    private IEnumerator DashCoroutine(float dashSpeed, float dashDuration)
    {
        isDashing = true;
        playerMovement.MovementOverride = true;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            // Apply dash velocity on XZ plane, maintain Y velocity for gravity
            Vector3 dashVelocity = dashDirection * dashSpeed;
            rb.linearVelocity = new Vector3(dashVelocity.x, rb.linearVelocity.y, dashVelocity.z);

            elapsed += Time.deltaTime;
            yield return null;
        }
        playerMovement.MovementOverride = false;
        isDashing = false;
    }

    // Public getter for external scripts
    public bool IsDashing => isDashing;
}