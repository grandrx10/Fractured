using Cards.Core.Behaviors;
using UnityEngine;
using Utils;

namespace Cards.Card_Assets.General_Behaviors
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Behaviors/Quest")]
    public class QuestItemBehavior: Behavior
    {
        public string itemName = "QuestItem";
    }
}