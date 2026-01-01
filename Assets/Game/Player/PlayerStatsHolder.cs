using System;
using Cards;
using Cards.Core.BehaviorTags;
using UnityEngine;

namespace Characters.Player
{
    [Serializable]
    public class PlayerStatsHolder
    {
        [SerializeReference] private PlayerStats baseStats;
        private PlayerStats _calculatedStats;
        private PlayerStats _tempStats;

        public PlayerStats GetStats()
        {
            
            return baseStats + _calculatedStats + _tempStats;
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
            if (!_tempStats) _tempStats = PlayerStats.Empty;
        }
    }
}