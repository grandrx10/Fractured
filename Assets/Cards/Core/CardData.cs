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
        public CardRarity rarity;
        public Sprite icon;
        // only for initialization!!!!
        public CardStats stats;
        public CardTier tier;
        public List<BaseBehavior> behaviors;
        public CardVisuals Visuals => new (){Name = name, FlavorText = flavorText, Rarity = rarity, Icon = icon};
    }

    public struct CardVisuals
    {
        public string Name;
        public string FlavorText;
        public CardRarity Rarity;
        public Sprite Icon;
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