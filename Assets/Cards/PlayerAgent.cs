using System;
using System.Collections.Generic;
using Cards.Core;
using Cards.Core.BehaviorTags;
using Cards.Visual;
using Characters;
using Characters.Player;
using UnityEngine;

namespace Cards
{
    public class PlayerAgent: Agent
    {
        public Card offHandCard;
        
        public PlayerStatsHolder stats;
        public Action OnStatsUpdate;

        private void Awake()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            if (FindObjectsByType<PlayerAgent>(FindObjectsSortMode.None).Length > 1) Destroy(gameObject);
        }

        public void UpdateStats()
        {
            stats.RecomputeStats(this);
            OnStatsUpdate?.Invoke();
        }
        
        public void SetMainHand(Card card)
        {
            if (card == selectedCard) return;

            if (card)
            {
                foreach (var hold in card.GetAllBehaviors<IBehaviorHoldListener>())
                {
                    hold.StartHold();
                }
            }

            if (selectedCard)
            {
                foreach (var hold in selectedCard.GetAllBehaviors<IBehaviorHoldListener>())
                {
                    hold.StopHold();
                }
            }
            
            selectedCard = card;
        }

        /*
         * Returns in order: main/off/other
         */
        public override Card SelectCardImmediate()
        {
            return RandomCard;
        }
    }
}