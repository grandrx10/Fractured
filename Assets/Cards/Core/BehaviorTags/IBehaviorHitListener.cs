using Cards.Environments;
using UnityEngine;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorHitListener
    {
        public void Hit(OpenWorldEnv env, GameObject target);
    }
}