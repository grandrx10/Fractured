using Cards.Environments;
using UnityEngine;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorPostHitListener
    {
        public void Hit(CardEnv env, Agent agent, GameObject target);
    }
}