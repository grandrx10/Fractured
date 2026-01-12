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
        public void Equip(PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.health -= hp;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public void Unequip(PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.health += hp;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public override string GetDescription()
        {
            return $"<b>Prickly</b>: When equipped, lose {hp} max health.";
        }
    }
}