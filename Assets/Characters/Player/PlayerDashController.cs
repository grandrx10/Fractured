using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerDashController : MonoBehaviour
{
    private bool isDashing = false;
    private Rigidbody rb;
    private Vector3 dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Gets the movement direction from WASD input on the XZ plane,
    /// relative to the camera's forward direction from PlayerInteractController
    /// </summary>
    private Vector3 GetInputDirection()
    {
        if (Characters.PlayerSingleton.Instance == null)
        {
            Debug.LogWarning("PlayerDashController: PlayerSingleton not found!");
            return Vector3.zero;
        }

        // Get the camera's forward direction from the PlayerInteractController
        Characters.PlayerInteractController interactController = 
            Characters.PlayerSingleton.Instance.GetComponent<Characters.PlayerInteractController>();

        if (interactController == null)
        {
            Debug.LogWarning("PlayerDashController: PlayerInteractController not found!");
            return Vector3.zero;
        }

        // Get the actual camera transform (raycastOrigin from PlayerInteractController)
        // If raycastOrigin is not accessible, fall back to Camera.main
        Transform cameraTransform = Camera.main.transform;

        // Get camera forward and right in world space
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Flatten camera directions to XZ plane
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

        // Create direction vector on XZ plane, relative to camera orientation
        Vector3 direction = cameraForward * vertical + cameraRight * horizontal;
        
        // Normalize to prevent faster diagonal movement
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

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            // Apply dash velocity on XZ plane, maintain Y velocity for gravity
            Vector3 dashVelocity = dashDirection * dashSpeed;
            rb.linearVelocity = new Vector3(dashVelocity.x, rb.linearVelocity.y, dashVelocity.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    // Public getter for external scripts
    public bool IsDashing => isDashing;
}