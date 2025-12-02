using UnityEngine;

namespace Cards.Core.Behaviors
{
    public class BaseUseBehavior: BaseBehavior
    {
        public virtual PhysicalCard Throw(PhysicalCard cardPrefab)
        {
            var c = Instantiate(cardPrefab);
            //Debug.Log("the card wil be throw");
            c.card = AttachedCard;
            return c;
        }
    }
}