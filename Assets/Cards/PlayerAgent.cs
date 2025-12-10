using System;
using System.Collections.Generic;
using Cards.Core;
using Cards.Visual;
using Characters;
using UnityEngine;

namespace Cards
{
    public class PlayerAgent: Agent
    {
        public Card mainHandCard;
        public Card offHandCard;

        public PlayerStatsHolder Stats {get; private set;}

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