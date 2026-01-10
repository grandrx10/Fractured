using Cards.Core.BehaviorTags;
using Cards.Environments;
using Cards.PhysicalProperties;
using Cards;
using UnityEngine;
using Utils;
using Characters;
using Game.Effects;

namespace Cards.Core.Behaviors
{
    [CreateAssetMenu(fileName = "JuicedUse", menuName = "Behaviors/JuicedUse")]
    public class JuicedUseBehavior : DefaultUseBehavior
    {
        [Header("Juiced Settings")]
        [PrefabComponent] public PhysicalObject juicedCardPrefab;
        public string description;
        public override bool Use(CardEnv env, Agent agent)
        {
            if (env is not OpenWorldEnv opEnv)
            {
                Debug.LogError("Env does not support throwing");
                return false;
            }
            
            bool isJuiced = env.HasEffect<JuicedEffect>();

            // Temporarily swap prefab if juiced
            PhysicalObject originalPrefab = cardPrefab;

            if (isJuiced && juicedCardPrefab != null)
            {
                Debug.Log("Is Juiced");
                cardPrefab = juicedCardPrefab;
            } else
            {
                Debug.Log("Not juiced");
            }

            // Throw card
            ThrowCard(agent, opEnv, Quaternion.identity);

            // Consume juiced status
            if (isJuiced)
            {
                env.RemoveEffect<JuicedEffect>();
            }

            // Restore original prefab
            cardPrefab = originalPrefab;

            return true;
        }

        public override string GetDescription()
        {
            return description;
        }
    }
}
