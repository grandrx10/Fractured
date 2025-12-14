using Cards.Environments;
using UnityEngine;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorPreHitListener
    {
        public void Hit(CardEnv env, Agent agent, GameObject target);
    }
}