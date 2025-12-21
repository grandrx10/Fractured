using UnityEngine;

namespace Cards.Core
{
    [CreateAssetMenu(fileName = "SuitCardData", menuName = "Card/SuitCardData")]
    public class TarotSuitCardData : CardData
    {
        public override CardVisuals GetVisuals(Card baseCard)
        {
            return new CardVisuals()
            {
                Name = $"{GetLevelName(baseCard.tier.level)} of {name}",
                FlavorText = flavorText,
                Style = style,
                Rarity = rarity,
                Icon = icon,
                CollectionName = GetCollectionName(collection),
            };
        }

        private static readonly string[] Names = {"Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King", "Ace"};

        private string GetLevelName(int t)
        {
            if (t < 0 || t > Names.Length) return "Illegal";
            return Names[t];
        }
    }
}