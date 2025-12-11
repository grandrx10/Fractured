using UnityEngine;

namespace Game.Health
{
    public class Health : MonoBehaviour
    {
        public int health;
        private int maxHealth;

        void Start()
        {
            maxHealth = health;
        }

        // Clamp health whenever it changes
        public virtual void takeDamage(int damage)
        {
            health -= damage;
            health = Mathf.Clamp(health, 0, maxHealth); // Ensure health stays within 0 and maxHealth
        }
    }
}
