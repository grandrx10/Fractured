using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;
using Utils;

namespace Cards.Card_Assets.General_Behaviors
{
    public class LookingAtBehavior: Behavior, IBehaviorUseListener
    {
        public LayerMask layers = ~0;
        public RaycastHit LookingAt(CardEnv env)
        {
            if (env is OpenWorldEnv opEnv)
            {
                return opEnv.GetPlayerLookTarget(layers);
            }
            return new();
        }

        public virtual bool Use(CardEnv env, Agent agent)
        {
            throw new System.NotImplementedException();
        }
    }
}