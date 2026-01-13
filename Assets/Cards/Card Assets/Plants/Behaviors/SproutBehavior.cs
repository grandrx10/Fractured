using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Characters.Player;
using UnityEngine;

namespace Cards.Card_Assets.Plants.Behaviors
{
    [CreateAssetMenu(fileName = "Sprout", menuName = "Behaviors/Sprout")]
    public class SproutBehavior : Behavior, IBehaviorUseListener
    {
        public string ability;
        public override string GetDescription()
        {
            return $"<b>{ability}</b>: On Use, heal {AttachedCard.stats.strength} health.";
        }

        public bool Use(CardEnv env, Agent agent)
        {
            if (env is RTCombatEnv cEnv)
            {
                return cEnv.Heal(AttachedCard.stats.strength);
            }
            return false;
        }
    }
}