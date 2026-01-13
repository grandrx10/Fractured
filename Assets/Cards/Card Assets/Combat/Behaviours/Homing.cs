using Game.Health;
using UnityEngine;

namespace Cards.Card_Assets.Combat.Behaviours
{
    [RequireComponent(typeof(Rigidbody))]
    public class Homing : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float speed = 10f;         // Forward speed
        public float turnSpeed = 5f;      // How fast it rotates toward the target

        private Rigidbody _rb;
        private Transform target;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = false;
            _rb.useGravity = false;
            _rb.linearVelocity = transform.forward * speed;
        }

        private void FixedUpdate()
        {
            if (target == null)
            {
                // Move straight if no target
                _rb.linearVelocity = transform.forward * speed;
                return;
            }

            // Direction toward target
            Vector3 direction = (target.position - transform.position).normalized;

            // Smoothly rotate toward target
            Vector3 newForward = Vector3.RotateTowards(transform.forward, direction, turnSpeed * Time.fixedDeltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newForward);

            // Apply forward velocity
            _rb.linearVelocity = transform.forward * speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Only target objects with Health
            var health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                target = other.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Lose target if it leaves the trigger
            if (other.transform == target)
            {
                target = null;
            }
        }
    }
}
