using Cards.Core.Behaviors;
using Cards.Environments;
using UnityEngine;

namespace Cards.Behaviors
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
        public override string GetDescription()
        {
            return $"<b>{type.ToString()}</b>: Yummy.";
        }
    }
}