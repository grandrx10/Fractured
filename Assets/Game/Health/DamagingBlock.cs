using System;
using UnityEngine;
using Utils;

namespace Game.Health
{
    public class DamagingBlock : MonoBehaviour
    {
        public int damage;
        private void OnCollisionEnter(Collision other)
        {
            if (PhysicsHelper.MainObj(other.collider).TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage, gameObject);
            }
        }
    }
}