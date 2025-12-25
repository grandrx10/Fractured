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
        
        public PlayerStatsHolder Stats {get; private set;}

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
        
        private void Awake()
        {
            Stats = FindFirstObjectByType<PlayerStatsHolder>();
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