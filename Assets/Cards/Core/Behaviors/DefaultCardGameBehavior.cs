using UnityEngine;

namespace Cards.Core.Behaviors
{
    public class DefaultCardGameBehavior: Behavior
    {
        public virtual (int, int) Collide(Card opponent)
        {
            Debug.Log("the card attacking");
            
            return (0, 0);
        }
    }
}