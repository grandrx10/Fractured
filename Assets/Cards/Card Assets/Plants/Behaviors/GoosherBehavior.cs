using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Characters.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cards.Card_Assets.Plants.Behaviors
{
    [CreateAssetMenu(fileName = "Goosher", menuName = "Behaviors/Goosher")]
    public class GoosherBehavior : Behavior, IBehaviorEquippedListener
    {
        public float jump = 2;
        public void Equip(Card card, PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.jumpPower += jump;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public void Unequip(Card card, PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.jumpPower -= jump;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public override string GetDescription(Card card)
        {
            return $"<b>(Passive) Windy</b>: Gives +{jump} jump power.";
        }
    }
}