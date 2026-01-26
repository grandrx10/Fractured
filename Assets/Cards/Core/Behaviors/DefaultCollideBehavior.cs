using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;

namespace Cards.Core.Behaviors
{
    [CreateAssetMenu(fileName = "Collide", menuName = "Behaviors/DefaultCollide")]
    public class DefaultCollideBehavior: Behavior, IBehaviorHitListener
    {
        public void Hit(Card card, OpenWorldEnv env, GameObject target)
        {
            Debug.Log("the card colided");
        }
    }
}