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
        public Card mainHandCard;
        public Card offHandCard;
        
        public PlayerStatsHolder Stats {get; private set;}

        public void SetMainHand(Card card)
        {
            if (card == mainHandCard) return;

            if (card)
            {
                foreach (var hold in card.GetAllBehaviors<IBehaviorHoldListener>())
                {
                    hold.StartHold();
                }
            }
            
            mainHandCard = card;
            foreach (var hold in mainHandCard.GetAllBehaviors<IBehaviorHoldListener>())
            {
                hold.StopHold();
            }
            
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