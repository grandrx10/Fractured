using System;
using System.Collections.Generic;
using Cards.Core;
using Cards.Visual;
using UnityEngine;

namespace Cards
{
    public class PlayerAgent: Agent
    {
        public Card mainHandCard;
        public Card offHandCard;
        public List<Card> deck;
        public HandLayout handLayout;

        private void Awake()
        {
            handLayout.CardUsed += card =>
            {
                SubmitCard(card);
            };
            handLayout.PopulateCards(hand);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && CardRequested)
            {
                handLayout.UseCard();
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                handLayout.OnScroll(Input.mouseScrollDelta.y);
            }
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