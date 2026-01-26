using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Characters.Player;
using UnityEngine;

namespace Cards.Card_Assets.Plants.Behaviors
{
    [CreateAssetMenu(fileName = "Bush", menuName = "Behaviors/Bush")]
    public class BushBehavior : Behavior, IBehaviorEquippedListener
    {
        public int hp = 1;
        public void Equip(Card card, PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.health -= hp;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public void Unequip(Card card, PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.health += hp;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public override string GetDescription(Card card)
        {
            return $"<b>(Passive) Prickly</b>: Lose {hp} max health.";
        }
    }
}