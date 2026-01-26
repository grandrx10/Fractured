using Cards.Core;
using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Card_Assets.General_Behaviors
{
    [CreateAssetMenu(fileName = "Temporary", menuName = "Behaviors/Temporary")]
    public class TemporaryBehavior : Behavior
    {
        public bool persistent;
        
        public override string GetDescription(Card card)
        {
            if (persistent) return "<b>(Innate) Disposable</b>: Is destroyed on use.";
            return "<b>(Innate) Fragile</b>: Is destroyed on moving between worlds.";
        }
    }
}