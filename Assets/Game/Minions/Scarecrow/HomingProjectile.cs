using UnityEngine;
using Cards.Environments;

namespace Game.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    public class HomingProjectile : MonoBehaviour
    {
        [Header("Homing Settings")]
        public float speed = 10f;
        public float maxTurnRate = 180f;    // Degrees per second

        [Header("Lifetime")]
        public float lifetime = 5f;

        private Rigidbody rb;
        private Transform player;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
        }

        private void Start()
        {
            // Cache the player reference
            if (OpenWorldEnv.Current != null)
            {
                player = OpenWorldEnv.Current.PlayerTransform;
            }

            // IMPORTANT: Use transform.forward to respect the spawned rotation
            rb.linearVelocity = transform.forward * speed;

            Destroy(gameObject, lifetime);
        }

        private void FixedUpdate()
        {
            if (player == null) return;

            // Calculate direction to player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            
            // Calculate desired velocity
            Vector3 desiredVelocity = directionToPlayer * speed;

            // Smoothly rotate current velocity towards desired velocity
            Vector3 newVelocity = Vector3.RotateTowards(
                rb.linearVelocity,
                desiredVelocity,
                maxTurnRate * Mathf.Deg2Rad * Time.fixedDeltaTime,
                0f
            );

            // Normalize and apply speed to maintain constant velocity
            rb.linearVelocity = newVelocity.normalized * speed;

            // Face the movement direction
            if (rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
            }
        }
    }
}