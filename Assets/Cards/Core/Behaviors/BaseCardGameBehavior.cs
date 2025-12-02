using UnityEngine;

namespace Cards.Core.Behaviors
{
    public class BaseCardGameBehavior: BaseBehavior
    {
        public virtual (int, int) Collide(Card opponent)
        {
            Debug.Log("the card attacking");
            
            return (0, 0);
        }
    }
}