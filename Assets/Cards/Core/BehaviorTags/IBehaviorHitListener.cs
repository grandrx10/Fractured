using Cards.Environments;
using UnityEngine;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorHitListener
    {
        public void Hit(Card card, OpenWorldEnv env, GameObject target);
    }
}