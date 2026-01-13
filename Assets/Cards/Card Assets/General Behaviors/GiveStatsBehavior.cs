using System;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Characters.Player;
using UnityEngine;

namespace Cards.Card_Assets.Plants.Behaviors
{
    [CreateAssetMenu(fileName = "GiveStats", menuName = "Behaviors/GiveStats")]
    public class GiveStatsBehavior : Behavior, IBehaviorEquippedListener
    {
        public PlayerStats.Stat stat;
        public bool negate;
        public float factor = 1;
        public string abilityName;
        public void Equip(PlayerAgent agent)
        {
            agent.stats.tempStats += PlayerStats.Single(stat, (negate ? -1 : 1) * AttachedCard.stats.strength * factor);
            agent.UpdateStats();
        }

        public void Unequip(PlayerAgent agent)
        {
            agent.stats.tempStats += PlayerStats.Single(stat, (negate ? 1 : -1) * AttachedCard.stats.strength * factor);
            agent.UpdateStats();
        }

        public override string GetDescription()
        {
            var n = negate ? "Lose" : "Gain";

            float raw = AttachedCard.stats.strength * factor;
            float rounded = (float)Math.Round(raw, 2);

            string valueStr = rounded.ToString("0.##");

            return $"<b>{abilityName}:</b> When equipped, {n} {valueStr} {PlayerStats.ToName(stat, false)}.";
        }
    }

    
}