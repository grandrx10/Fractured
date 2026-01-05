using Cards.Core.Behaviors;
using Game;
using UnityEngine;

namespace Cards.Card_Assets.Systems.B
{
    [CreateAssetMenu(fileName = "Book", menuName = "Behaviors/Book")]
    public class BookBehavior : Behavior
    {
        [TextArea(2, 5)] public string text;
        public override string GetDescription()
        {
            return text;
        }
    }
}