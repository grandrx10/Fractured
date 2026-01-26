using Cards.Core;
using Cards.Core.Behaviors;
using UnityEngine;
using Utils;

namespace Cards.Card_Assets.General_Behaviors
{
    [CreateAssetMenu(fileName = "Quest Item", menuName = "Behaviors/Quest Item")]
    public class QuestItemBehavior: Behavior
    {
        public string itemName = "QuestItem";

        public override string GetDescription(Card card)
        {
            return "<b>Quest Item</b>";
        }
    }
}