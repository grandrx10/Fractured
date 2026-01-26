using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;

namespace Cards.Card_Assets.Combat.Behaviours
{
    [CreateAssetMenu(fileName = "CursedShield", menuName = "Behaviors/CursedShield")]
    public class CursedShieldBehavior: Behavior, IBehaviorCombatListener, IBehaviorHasStateTag
    {
        public string abilityName;
        public virtual void StartMatch(Card card, RTCombatEnv env)
        {
           env.AddShield(card.stats.strength);
        }
        
        public virtual void EndMatch(Card card, RTCombatEnv env)
        {
            
        }
        
        public override string GetDescription(Card card)
        {
            return $"<b>(Passive) {abilityName}:</b> Grants {card.stats.strength} shield at the start of combat.";
        }

        public Card AttachedCard { get; set; }
    }
}