using Cards.Core.Behaviors;
using Game;
using UnityEngine;

namespace Cards.Card_Assets.Systems.B
{
    [CreateAssetMenu(fileName = "Money", menuName = "Behaviors/Money")]
    public class MoneyBehavior : Behavior
    {
        public int value;
        public override string GetDescription()
        {
            string s = $"Value: {value}";
            return s;
        }
    }
}