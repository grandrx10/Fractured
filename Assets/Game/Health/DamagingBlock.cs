using System;
using UnityEngine;
using Utils;

namespace Game.Health
{
    public class DamagingBlock : MonoBehaviour
    {
        public int damage = 1;
        private void OnCollisionEnter(Collision other)
        {
            if (PhysicsHelper.MainObj(other.collider).TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage, gameObject);
                Debug.Log("Yeah taking damage");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (PhysicsHelper.MainObj(other).TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage, gameObject);
                Debug.Log("Yeah taking damage");
            }
        }
    }
}