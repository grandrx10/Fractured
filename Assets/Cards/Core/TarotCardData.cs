using System;
using System.Collections.Generic;
using Cards.Core.Behaviors;
using Cards.Visual;
using UnityEngine;

namespace Cards.Core
{
    [CreateAssetMenu(fileName = "TarotCardData", menuName = "Card/TarotCardData")]
    public class TarotCardData : CardData
    {
        public CardDisplayPrefab customDisplay;
    }
}