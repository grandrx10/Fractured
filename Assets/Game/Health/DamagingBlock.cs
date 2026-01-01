using System;
using UnityEngine;
using Utils;

namespace Game.Health
{
    public class DamagingBlock : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (PhysicsHelper.MainObj(other.collider).TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(1, gameObject);
            }
        }
    }
}