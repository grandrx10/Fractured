using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Characters.Player;
using UnityEngine;

namespace Cards.Card_Assets.Plants.Behaviors
{
    [CreateAssetMenu(fileName = "SpringberryBush", menuName = "Behaviors/SpringberryBush")]
    public class SpringberryBushBehavior : Behavior, IBehaviorEquippedListener
    {
        public void Equip(PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.speed += AttachedCard.stats.strength;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public void Unequip(PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.speed -= AttachedCard.stats.strength;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public override string GetDescription()
        {
            return $"<b>Springy</b>: When equipped, gives +{AttachedCard.stats.strength} movement speed.";
        }
    }
}