using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;

namespace Cards.Behaviors
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Behaviors/Projectile")]
    public class ProjectileBehavior : Behavior, IBehaviorUseListener
    {
        public GameObject prefab;
        public string abilityName, objectName;
        public void Use(CardEnv env, Agent agent)
        {
            //TODO: make a base env that defines throwing cards
            if (env is OpenWorldEnv opEnv)
            {
                
            }
            else
            {
                Debug.LogError("Env Does not support throwing");
            }
        }
        
        public override string GetDescription()
        {
            return $"<b>{abilityName}</b>: On Use, throws a {objectName}.";
        }
    }
}