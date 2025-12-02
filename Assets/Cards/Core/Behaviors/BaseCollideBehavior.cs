using UnityEngine;

namespace Cards.Core.Behaviors
{
    public class BaseCollideBehavior: BaseBehavior
    {
        public virtual void Collide()
        {
            Debug.Log("the card colided");
        }
    }
}