using System.Collections.Generic;
using UnityEngine;

namespace Cards.Card_Assets.Combat.Behaviours
{
    public class Barrier : MonoBehaviour
    {
        [Header("Lifetime Settings")]
        public float defaultLifetime = 1f;
        public float maxLifetime = 2.5f;

        private float lifetimeRemaining;
        private int extensionStep = 0; // How many times we've extended
        private readonly float[] extensionValues = { 0.5f, 0.4f, 0.3f, 0.2f, 0.1f };

        private HashSet<GameObject> hitProjectiles = new HashSet<GameObject>();

        private void Start()
        {
            lifetimeRemaining = defaultLifetime;
        }

        private void Update()
        {
            lifetimeRemaining -= Time.deltaTime;
            if (lifetimeRemaining <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            GameObject other = collision.gameObject;

            // Only consider objects in the "Projectile" layer
            if (other.layer == LayerMask.NameToLayer("Projectile"))
            {
                // Only extend if this projectile hasn't already triggered it
                if (!hitProjectiles.Contains(other))
                {
                    hitProjectiles.Add(other);
                    ExtendLifetime();
                }
            }
        }

        private void ExtendLifetime()
        {
            if (extensionStep < extensionValues.Length)
            {
                lifetimeRemaining += extensionValues[extensionStep];
                extensionStep++;

                // Clamp to maxLifetime
                if (lifetimeRemaining > maxLifetime)
                    lifetimeRemaining = maxLifetime;
            }
        }
    }
}
