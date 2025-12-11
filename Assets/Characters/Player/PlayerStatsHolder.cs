using Cards;
using Cards.Core.BehaviorTags;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerStatsHolder : MonoBehaviour
    {
        [SerializeReference] private PlayerStats baseStats;
        private PlayerStats _calculatedStats;
        private PlayerStats _tempStats;
        private PlayerAgent _playerAgent;

        public PlayerStats GetStats()
        {
            return baseStats + _calculatedStats + _tempStats;
        }
        
        private void Awake()
        {
            _playerAgent = FindFirstObjectByType<PlayerAgent>();
        }

        public void RecomputeStats()
        {
            _calculatedStats = PlayerStats.Empty;
            foreach (var card in _playerAgent.hand)
            {
                foreach (var stats in card.GetAllBehaviors<IBehaviorStatUpdater>())
                {
                    _calculatedStats += stats.GetStats();
                }
            }
        }
    }
}