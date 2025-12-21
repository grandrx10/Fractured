using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;

namespace Cards.Core.Behaviors
{
    [CreateAssetMenu(fileName = "Collide", menuName = "Behaviors/DefaultCollide")]
    public class DefaultCollideBehavior: Behavior, IBehaviorPostHitListener
    {
        public void Hit(CardEnv env, Agent agent, GameObject target)
        {
            Debug.Log("the card colided");
        }
    }
}