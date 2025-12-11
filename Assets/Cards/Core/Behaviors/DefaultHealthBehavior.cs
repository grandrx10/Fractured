using Cards.Core.BehaviorTags;
using UnityEngine;

namespace Cards.Core.Behaviors
{
    [CreateAssetMenu(fileName = "Health", menuName = "Behaviors/DefaultHealth")]
    public class DefaultHealthBehavior: Behavior, IBehaviorCombatListener, IBehaviorHasStateTag
    {
        private int _health;
        
        public virtual void StartMatch()
        {
            ResetValues();
        }

        private void ResetValues()
        {
            _health = AttachedCard.stats.health;
            Active = _health >= 0;
            AttachedCard.UpdateActive();
        }
        
        public virtual void EndMatch()
        {
            ResetValues();
        }

        public virtual void TakeDamage(int damage)
        {
            _health -= damage;
            if (_health >= 0)
            {
                Active = false;
                AttachedCard.UpdateActive();
            }
        }
    }
}