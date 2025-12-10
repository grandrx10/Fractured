using UnityEngine;

namespace Cards.PhysicalProperties
{
    [RequireComponent(typeof(Rigidbody))]
    public class CurveTo : MonoBehaviour
    {
        [Header("Flight Settings")]
        public float initialForwardDistance = 5f; // Distance to move forward from spawn
        public float curveDuration = 0.25f;       // Time to move toward midPoint before physics
        public float straightSpeed = 25f;         // Speed when physics takes over
        public float midPointDistance = 3f;       // Units in front of player for midPoint
        public float midPointHeight = 2f;         // Height offset for the arc
        public float rotationSpeed = 10f;         // How fast the card rotates toward target

        private Vector3 finalTarget;
        private Vector3 midPoint;
        private Vector3 forwardPosition;          // Position after initial forward movement
        private Vector3 spawnPosition;            // Original spawn position
        private Vector3 spawnForward;             // Direction card was facing at spawn
        private Vector3 lastPosition;             // Track position for velocity calculation
        private float timer = 0f;

        private Rigidbody rb;
        private bool physicsActive = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Start()
        {
            // Store spawn info
            spawnPosition = transform.position;
            spawnForward = transform.forward;
            lastPosition = spawnPosition;
        
            // Calculate forward position (after initial forward movement)
            forwardPosition = spawnPosition + spawnForward * initialForwardDistance;
        
            // Get player singleton
            PlayerSingleton player = PlayerSingleton.Instance;
            if (player == null)
            {
                Debug.LogError("PlayerSingleton.Instance not found!");
                return;
            }

            // Get interact controller
            PlayerInteractController pic = player.GetComponent<PlayerInteractController>();
            if (pic == null)
            {
                Debug.LogError("PlayerInteractController not found on PlayerSingleton!");
                return;
            }

            // Final target
            finalTarget = pic.GetCameraRaycastTarget();

            // MidPoint: in front of player toward final target, with height offset
            Vector3 playerPos = player.transform.position;
            Vector3 towardTarget = (finalTarget - playerPos).normalized;
            midPoint = playerPos + towardTarget * midPointDistance + Vector3.up * midPointHeight;
        
            // Start with kinematic control
            rb.isKinematic = true;
            timer = 0f;
        }

        void Update()
        {
            if (!physicsActive)
            {
                // --- Phase 1: move through curve ---
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / curveDuration);
            
                // Use Catmull-Rom spline
                // p0: extrapolated point behind spawn for smooth entry
                Vector3 p0 = spawnPosition - spawnForward * initialForwardDistance;
                Vector3 p1 = spawnPosition;     // Start at spawn
                Vector3 p2 = forwardPosition;   // Move forward
                Vector3 p3 = midPoint;          // Then to midpoint
                Vector3 p4 = finalTarget;       // Finally toward target
            
                // We need to interpolate through multiple segments
                Vector3 curvePosition;
                if (t < 0.5f)
                {
                    // First half: spawn to forward position to midpoint
                    float localT = t * 2f; // Remap to 0-1
                    curvePosition = CatmullRom(p0, p1, p2, p3, localT);
                }
                else
                {
                    // Second half: forward position to midpoint to target
                    float localT = (t - 0.5f) * 2f; // Remap to 0-1
                    curvePosition = CatmullRom(p1, p2, p3, p4, localT);
                }
            
                // Calculate movement direction from actual position change
                Vector3 movementDirection = curvePosition - lastPosition;
            
                // Rotate toward movement direction
                if (movementDirection.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(movementDirection.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }
            
                // Update position after calculating movement
                transform.position = curvePosition;
                lastPosition = curvePosition;

                // Activate physics after curveDuration
                if (timer >= curveDuration)
                {
                    ActivatePhysics();
                }
            }
            else
            {
                // Phase 2: align rotation with velocity
                if (rb.linearVelocity.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(rb.linearVelocity);
                    rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.deltaTime));
                }
            }
        }

        private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            // Catmull-Rom spline interpolates between p1 and p2
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * (
                (2f * p1) +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                (-p0 + 3f * p1 - 3f * p2 + p3) * t3
            );
        }

        private void ActivatePhysics()
        {
            physicsActive = true;
            rb.isKinematic = false;

            // Shoot straight toward final target
            Vector3 dir = (finalTarget - transform.position).normalized;
            rb.linearVelocity = dir * straightSpeed;
        }
    }
}