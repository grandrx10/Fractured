using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Card_Assets.General_Behaviors
{
    [CreateAssetMenu(fileName = "Temporary", menuName = "Behaviors/Temporary")]
    public class TemporaryBehavior : Behavior
    {
        public bool persistent;
        
        public override string GetDescription()
        {
            if (persistent) return "<b>Disposable</b>: Is destroyed on use.";
            return "<b>Fragile</b>: Is destroyed on use or on moving between worlds.";
        }
    }
}