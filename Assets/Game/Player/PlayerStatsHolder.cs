using System;
using Cards;
using Cards.Core.BehaviorTags;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Player
{
    [Serializable]
    public class PlayerStatsHolder
    {
        [SerializeReference] private PlayerStats baseStats;
        private PlayerStats _calculatedStats;
        public PlayerStats tempStats;
        
        public PlayerStats GetStats()
        {
            return baseStats + _calculatedStats + tempStats;
        }

        public void RecomputeStats(Agent player)
        {
            _calculatedStats = PlayerStats.Empty;
            foreach (var card in player.hand)
            {
                foreach (var stats in card.GetAllBehaviors<IBehaviorStatUpdater>())
                {
                    _calculatedStats += stats.GetStats();
                }
            }
            if (!tempStats) tempStats = PlayerStats.Empty;
        }
    }
}