using Cards.Core.BehaviorTags;
using Cards.Environments;
using Cards.PhysicalProperties;
using Cards;
using UnityEngine;
using Utils;

namespace Cards.Core.Behaviors
{
    [CreateAssetMenu(fileName = "Use", menuName = "Behaviors/DefaultUse")]
    public class DefaultUseBehavior: Behavior, IBehaviorUseListener
    {
        [PrefabComponent] public PhysicalObject cardPrefab;
        public float speed;
        public virtual bool Use(CardEnv env, Agent agent)
        {
            if (env is OpenWorldEnv opEnv)
            {
                ThrowCard(agent, opEnv, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Env Does not support throwing");
            }

            return true;
        }
        
        public void ThrowCard(Agent player, OpenWorldEnv env, Quaternion rotation)
        {
            var p = player.transform.position;
            var pLook = rotation * player.transform.forward;
            var d = rotation * env.PlayerLook;
            var c = Instantiate(cardPrefab, p, Quaternion.LookRotation(d));
            c.card = AttachedCard;
            c.InitState = new PhysicalObject.PhysicalInitState()
            {
                CenterPosition = p,
                StartDirection = pLook,
                StartPosition = p + pLook,
                TargetDirection = d,
                Speed = speed,
                Target = player.transform,
            };
            //c.GetComponent<Rigidbody>().linearVelocity = d;
        }
    }
}