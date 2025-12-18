using Cards.Environments;
using UnityEngine;

namespace Game.Health
{
    public class PlayerHealth : MonoBehaviour
    {
        public RTCombatEnv env;
        void Start()
        {
            env = FindAnyObjectByType<RTCombatEnv>();
        }

        // Clamp health whenever it changes
        public virtual bool TakeDamage(float damage)
        {
            return env.TakeDamage(damage);
        }
    }
}
