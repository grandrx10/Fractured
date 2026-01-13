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
        public virtual void StartMatch(RTCombatEnv env)
        {
           env.AddShield(AttachedCard.stats.strength);
        }
        
        public virtual void EndMatch(RTCombatEnv env)
        {
            
        }
        
        public override string GetDescription()
        {
            return $"<b>{abilityName}:</b> grants {AttachedCard.stats.strength} shield at the start of combat.";
        }
    }
}