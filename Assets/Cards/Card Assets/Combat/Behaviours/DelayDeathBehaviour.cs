using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Game.Health;
using UnityEngine;

namespace Cards.Card_Assets.Combat.Behaviours
{
    [CreateAssetMenu(menuName = "Behaviors/Delayed Death")]
    public class DelayDeathBehavior : Behavior, IBehaviorTakeDamageListener, IBehaviorHasStateTag
    {
        public float Priority => -100;
        public int saveCap = 5, explode = 8;
        private float _saved;
        public PlayerDamageData Hit(OpenWorldEnv env, Agent agent, PlayerDamageData data)
        {
            _saved += data.Damage;
            if (_saved < saveCap)
            {
                data.Damage = 0;
                data.ForceIframes = true;
                Debug.Log("less damage");
            }
            else
            {
                _saved = 0;
                data.Damage = explode;
                Debug.Log("Exploded");
            }
            return data;
        }
    }
}
