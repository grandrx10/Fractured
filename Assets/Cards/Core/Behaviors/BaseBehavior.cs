using UnityEngine;

namespace Cards.Core.Behaviors
{
    public class BaseBehavior: ScriptableObject
    {
        protected Card AttachedCard;
        public bool Active {get; set;}
        public virtual void Init(Card card)
        {
            AttachedCard = card;
        }
    }
}