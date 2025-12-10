using Cards.Core.Behaviors;
using Cards.Environments;
using UnityEngine;

namespace Cards.Behaviors
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