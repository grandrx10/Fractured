using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Characters.Player;
using UnityEngine;

namespace Cards.Card_Assets.Plants.Behaviors
{
    [CreateAssetMenu(fileName = "DirectHeal", menuName = "Behaviors/DirectHeal")]
    public class DirectHealBehavior : Behavior, IBehaviorUseListener
    {
        public string ability;
        public override string GetDescription(Card card)
        {
            return $"<b>(Active) {ability}</b>: Heal {card.stats.strength} health.";
        }

        public bool Use(Card card, CardEnv env, Agent agent)
        {
            if (env is RTCombatEnv cEnv)
            {
                return cEnv.Heal(card.stats.strength);
            }
            return false;
        }
    }
}