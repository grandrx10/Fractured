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
        public void Equip(PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.jumpPower += jump;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public void Unequip(PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.jumpPower -= jump;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public override string GetDescription()
        {
            return $"<b>Windy</b>: In hand, gives +{jump} jump power.";
        }
    }
}