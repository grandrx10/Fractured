using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Characters.Player;
using UnityEngine;

namespace Cards.Card_Assets.Plants.Behaviors
{
    [CreateAssetMenu(fileName = "SpringberryBush", menuName = "Behaviors/SpringberryBush")]
    public class SpringberryBushBehavior : Behavior, IBehaviorEquippedListener
    {
        public void Equip(Card card, PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.speed += card.stats.strength;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public void Unequip(Card card, PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.speed -= card.stats.strength;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public override string GetDescription(Card card)
        {
            return $"<b>(Passive) Springy</b>: Gives +{card.stats.strength} movement speed.";
        }
    }
}