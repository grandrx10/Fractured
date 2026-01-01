using Cards.Environments;
using UnityEngine;

namespace Game.Health
{
    public class PlayerHealth : MonoBehaviour
    {
        public RTCombatEnv env;
        private bool _initialized;
        void Start()
        {
            if (!_initialized) GlobalWorldManager.OnLoadNewScene += Init;
        }

        public void Init(CardEnv environment)
        {
            _initialized = true;
            env = environment as RTCombatEnv;
            GlobalWorldManager.OnLoadNewScene -= Init;
        }
        
        public virtual bool TakeDamage(float damage, GameObject instigator)
        {
            return env.TakeDamage(new PlayerDamageData()
            {
                Damage = damage,
                Target = this,
                Attacker = instigator
            });
        }
    }
    
    public struct PlayerDamageData
    {
        public float Damage;
        public PlayerHealth Target;
        public GameObject Attacker;
        public bool ForceIframes;
    }
}
