using UnityEngine;

namespace Cards.Core.Behaviors
{
    public class Behavior: ScriptableObject
    {
        protected Card AttachedCard;
        public bool Active {get; set;}
        public virtual void Init(Card card)
        {
            AttachedCard = card;
        }

        public virtual string GetDescription()
        {
            return "";
        }
        
        public virtual GameObject GetMenuObject()
        {
            return null;
        }
    }
}