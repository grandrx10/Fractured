using System;
using System.Collections.Generic;
using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Core
{
    [CreateAssetMenu(fileName = "CardData", menuName = "Card/CardData")]
    public class CardData : ScriptableObject
    {
        // should be fixed
        public new string name;
        public string flavorText;
        public CardCollection collection;
        public CardStyle style;
        public Sprite icon;
        // only for initialization!!!!
        public CardStats stats;
        public List<Behavior> behaviors;
        public virtual CardVisuals GetVisuals(Card baseCard)
        {
            return new CardVisuals()
            {
                Name = name,
                FlavorText = flavorText,
                Style = style,
                Icon = icon,
                CollectionName = GetCollectionName(collection),
            };
        }
        
        public string GetCollectionName(CardCollection c)
        {
            switch (collection)
            {
                case CardCollection.TarotTrump:
                    return "Tarot-Trump";
                case CardCollection.TarotSuit:
                    return "Tarot-Suits";
                case CardCollection.RPS:
                    return "RPS";
                case CardCollection.Plants:
                    return "Plants";
                case CardCollection.Tools:
                    return "Tools";
                case CardCollection.Fruits:
                    return "Fruits";
                case CardCollection.SetTheory:
                    return "Set Theory";
                case CardCollection.Elements:
                    return "Elements";
                case CardCollection.Common:
                    return "Common";
                case CardCollection.Books:
                    return "Books";
                default:
                    return "huh";
            }
        }
    }

    public struct CardVisuals
    {
        public string Name;
        public string FlavorText;
        public string CollectionName;
        public CardStyle Style;
        public Sprite Icon;
    }
    
    [Serializable]
    public enum CardStyle
    {
        Standard,
        Tarot,
        FullText,
        FullSprite
    }
    
    [Serializable]
    public enum CardCollection
    {
        TarotTrump,
        TarotSuit,
        RPS,
        Plants,
        Tools,
        Fruits,
        SetTheory,
        Elements,
        Common,
        Books
    }

    [Serializable]
    public struct CardStats
    {
        public int health;
        public int mana;
        public int strength;
    }
    
    [Serializable]
    public struct CardTier
    {
        public CardRank rank;
        public int level;
    }
    
    public enum CardRarity {
        Green,
        Blue,
        Purple,
        Orange,
    }
    
    public enum CardRank {
        None,
        Plus,
        PlusPlus
    }
}