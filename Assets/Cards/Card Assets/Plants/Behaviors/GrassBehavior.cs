using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Characters.Player;
using UnityEngine;

namespace Cards.Card_Assets.Plants.Behaviors
{
    [CreateAssetMenu(fileName = "Grass", menuName = "Behaviors/Grass")]
    public class GrassBehavior : Behavior, IBehaviorEquippedListener, IBehaviorTickListener, IBehaviorCombatListener
    {
        public int time = 20;
        public int healthCost = 4;
        private float _t;
        public void Equip(Card card, PlayerAgent agent)
        {
            agent.stats.tempStats += PlayerStats.Single(PlayerStats.Stat.Health, -healthCost);
            agent.UpdateStats();
        }

        public void Unequip(Card card, PlayerAgent agent)
        {
            agent.stats.tempStats += PlayerStats.Single(PlayerStats.Stat.Health, -healthCost);
            agent.UpdateStats();
        }

        public override string GetDescription(Card card)
        {
            return $"<b>(Passive) Shallow Roots</b>: Lose {healthCost} max health, but recover 1 health every {time} seconds.";
        }

        public void Tick(Card card, CardEnv env, Agent agent)
        {
            if (env is RTCombatEnv rtc)
            {
                _t += Time.deltaTime;
                if (_t >= time)
                {
                    _t -= time;
                    rtc.Heal(1);
                }
            }
        }

        public void StartMatch(Card card, RTCombatEnv env)
        {
            _t = 0;
        }

        public void EndMatch(Card card, RTCombatEnv env)
        {
            
        }
    }
}