using Cards.Environments;
using UnityEngine;

namespace Cards.Core.Behaviors
{
    public class BaseUseBehavior: BaseBehavior
    {
        public virtual void Throw(CardEnv env, Agent agent)
        {
            //TODO: make a base env that defines throwing cards
            if (env is OpenWorldEnv opEnv)
            {
                opEnv.ThrowCard(AttachedCard, Quaternion.identity, 30);
            }
            else
            {
                Debug.LogError("Env Does not support throwing");
            }
        }
    }
}