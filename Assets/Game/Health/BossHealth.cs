using UnityEngine;

namespace Game.Health
{
    public class BossHealth : Health
    {
        [Header("Boss Settings")]
        public bool isInvincible = false;

        private int playerProjectileLayer;

        protected override void Start()
        {
            base.Start();
            playerProjectileLayer = LayerMask.NameToLayer("PlayerProjectile");
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleHit(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleHit(other.gameObject);
        }

        private void HandleHit(GameObject other)
        {
            // Ignore anything not on PlayerProjectile layer
            if (other.layer != playerProjectileLayer)
                return;

            if (isInvincible)
            {
                InvincibleReaction(other);
                return;
            }

            // To be implemented at a later date.
            // TakeDamage(collisionDamage);
        }

        protected virtual void InvincibleReaction(GameObject source)
        {
            Debug.Log("Boss is invincible!");
        }
    }
}
