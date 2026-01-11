using UnityEngine;

namespace Game.Health
{
    public class Health : MonoBehaviour
    {
        public float health;
        protected int maxHealth;

        protected virtual void Start()
        {
            maxHealth = (int)health;
        }

        public virtual void TakeDamage(int damage)
        {
            health -= damage;
            health = Mathf.Clamp(health, 0, maxHealth);
        }
    }
}
