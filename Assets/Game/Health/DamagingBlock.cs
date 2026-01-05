using System;
using UnityEngine;
using Utils;

namespace Game.Health
{
    public class DamagingBlock : MonoBehaviour
    {
        public int damage;
        private void OnCollisionStay(Collision other)
        {
            if (PhysicsHelper.MainObj(other.collider).TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage, gameObject);
                Debug.Log("Yeah taking damage");
            }
        }
    }
}