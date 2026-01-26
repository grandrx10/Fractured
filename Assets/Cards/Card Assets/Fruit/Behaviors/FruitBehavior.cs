using Cards.Core;
using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Card_Assets.Fruit.Behaviors
{
    [CreateAssetMenu(fileName = "Fruit", menuName = "Behaviors/Fruit")]
    public class FruitBehavior : Behavior
    {
        public FruitType type;

        public enum FruitType
        {
            Apple,
            Orange,
            Lemon,
            Pepper,
            Tomato,
            Lychee,
            Carrot
        }
        public override string GetDescription(Card card)
        {
            return $"<b>{type.ToString()}</b>: Yummy.";
        }
    }
}