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
        public PlayerStatsHolder stats;
        public Action OnStatsUpdate;

        private void Awake()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            if (FindObjectsByType<PlayerAgent>(FindObjectsSortMode.None).Length > 1) Destroy(gameObject);
            OnAddCard += (card, target) =>
            {
                if (target == CardTarget.Deck) return;
                foreach (var equip in card.GetAllBehaviors<IBehaviorEquippedListener>())
                {
                    equip.Equip(this);
                }
            };
            OnRemoveCard += (card, target) =>
            {
                if (target == CardTarget.Deck) return;
                foreach (var equip in card.GetAllBehaviors<IBehaviorEquippedListener>())
                {
                    equip.Unequip(this);
                }
            };
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
            UpdateStats();
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