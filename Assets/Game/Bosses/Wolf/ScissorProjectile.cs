using UnityEngine;
using Characters;
using Cards.Card_Assets.RPS.Behaviors;
using Cards.Environments;

namespace Game.Bosses
{
    [RequireComponent(typeof(Rigidbody))]
    public class ScissorProjectile : MonoBehaviour
    {
        [Header("Homing Settings")]
        public float homingStrength = 5f;
        public float maxTurnRate = 180f; // degrees per second
        public float lifetime = 10f;
        
        private Rigidbody rb;
        private Transform bossTransform;
        private float currentAngle;
        private float orbitRadius;
        private float orbitSpeed;
        private bool isOrbiting = true;
        private bool isLaunched = false;
        private float currentSpeed;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        public void Initialize(Transform boss, float startAngle, float radius, float speed)
        {
            bossTransform = boss;
            currentAngle = startAngle;
            orbitRadius = radius;
            orbitSpeed = speed;
            isOrbiting = true;
            isLaunched = false;
        }

        public void Launch(float speed)
        {
            isOrbiting = false;
            isLaunched = true;
            currentSpeed = speed;
            rb.isKinematic = false;
            
            // Set initial velocity in current forward direction
            rb.linearVelocity = transform.forward * speed;
            
            // Destroy after lifetime
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (isOrbiting && bossTransform != null)
            {
                OrbitBoss();
            }
        }

        private void FixedUpdate()
        {
            if (isLaunched)
            {
                HomeTowardsPlayer();
            }
        }

        private void OrbitBoss()
        {
            // Update angle
            currentAngle += orbitSpeed * Time.deltaTime;
            if (currentAngle >= 360f) currentAngle -= 360f;
            
            // Calculate position
            Vector3 offset = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * orbitRadius;
            transform.position = bossTransform.position + offset;
            
            // Point outward
            transform.rotation = Quaternion.LookRotation(offset.normalized);
        }

        private void HomeTowardsPlayer()
        {
            Transform player = OpenWorldEnv.Current.PlayerTransform;
            
            // Calculate direction to player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            
            // Calculate desired velocity direction
            Vector3 desiredVelocity = directionToPlayer * currentSpeed;
            
            // Smoothly turn towards desired direction
            Vector3 newVelocity = Vector3.RotateTowards(
                rb.linearVelocity,
                desiredVelocity,
                maxTurnRate * Mathf.Deg2Rad * Time.fixedDeltaTime,
                0f
            );
            
            // Apply homing force
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, newVelocity, homingStrength * Time.fixedDeltaTime);
            
            // Update rotation to face movement direction
            if (rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
            }
        }

        private void OnDestroy()
        {
            // Clean up if needed
        }


    }
}