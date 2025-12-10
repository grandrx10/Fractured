using UnityEngine;

namespace Cards.Core.Behaviors
{
    public class DefaultCollideBehavior: Behavior
    {
        public virtual void Collide()
        {
            Debug.Log("the card colided");
        }
    }
}