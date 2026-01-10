using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Characters.Player;
using UnityEngine;

namespace Cards.Card_Assets.Plants.Behaviors
{
    [CreateAssetMenu(fileName = "SpringberryBush", menuName = "Behaviors/SpringberryBush")]
    public class SpringberryBushBehavior : Behavior, IBehaviorEquippedListener
    {
        public float speed = 2;
        public void Equip(PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.speed += speed;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public void Unequip(PlayerAgent agent)
        {
            var s = PlayerStats.Empty;
            s.speed -= speed;
            agent.stats.tempStats += s;
            agent.UpdateStats();
        }

        public override string GetDescription()
        {
            return $"<b>Springy</b>: In hand, gives +{speed} movement speed.";
        }
    }
}