using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;

namespace Cards.Core.Behaviors
{
    [CreateAssetMenu(fileName = "Health", menuName = "Behaviors/DefaultHealth")]
    public class DefaultHealthBehavior: Behavior, IBehaviorCombatListener, IBehaviorHasStateTag
    {
        private int _health;
        
        public virtual void StartMatch(Card card, RTCombatEnv env)
        {
            ResetValues();
        }

        private void ResetValues()
        {
            //_health = AttachedCard.stats.health;
            //Active = _health >= 0;
            //AttachedCard.UpdateActive();
        }
        
        public virtual void EndMatch(Card card, RTCombatEnv env)
        {
            ResetValues();
        }

        public virtual void TakeDamage(Card card, int damage)
        {
            _health -= damage;
            if (_health >= 0)
            {
                Active = false;
                card.UpdateActive();
            }
        }

        public Card AttachedCard { get; set; }
    }
}