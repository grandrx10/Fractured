using UnityEngine;

namespace Cards.Core.Behaviors
{
    public abstract class Behavior: ScriptableObject
    {
        public bool Active {get; set;}

        public virtual string GetDescription(Card card)
        {
            return "";
        }
    }
}