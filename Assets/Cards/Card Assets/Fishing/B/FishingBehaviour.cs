using Cards.Core;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Cards.PhysicalProperties;
using Cards.Core.Behaviors;
using UnityEngine;
using Characters;

namespace Cards.Card_Assets.Fishing.B
{
    [CreateAssetMenu(fileName = "Use", menuName = "Behaviors/Fishing")]
    public class FishingBehaviour : Behavior, IBehaviorUseListener, IBehaviorHasStateTag
    {
        public GameObject fishingRodPrefab;
        public string state = "ready";
        private GameObject spawnedRod;

        public virtual bool Use(Card card, CardEnv env, Agent agent)
        {
            if (env is OpenWorldEnv opEnv)
            {
                if (state == "ready")
                {
                    SpawnRod(agent, opEnv, Quaternion.identity);
                    state = "cast";
                }
                else if (state == "cast")
                {
                    Destroy(spawnedRod);
                    state = "ready";
                }
            }
            else
            {
                Debug.LogError("Env does not support fishing");
            }

            return true;
        }

        public GameObject SpawnRod(Agent player, OpenWorldEnv env, Quaternion rotation)
        {
            if (fishingRodPrefab == null)
                return null;

            Transform playerTransform = OpenWorldEnv.Current.PlayerTransform;

            Vector3 spawnPos = playerTransform.position;
            Quaternion spawnRot = playerTransform.rotation;

            spawnedRod = Object.Instantiate(fishingRodPrefab, spawnPos, spawnRot);

            spawnedRod.transform.SetParent(playerTransform, worldPositionStays: false);
            spawnedRod.transform.localPosition = Vector3.zero;
            spawnedRod.transform.localRotation = Quaternion.identity;

            Vector3 lookDir = rotation * env.PlayerLook;

            FishingRod rod = spawnedRod.GetComponent<FishingRod>();
            if (rod != null)
            {
                rod.Initialize(lookDir, this);
            }
            else
            {
                Debug.LogWarning("Spawned fishing rod has no FishingRod component");
            }

            return spawnedRod;
        }

        // Called when minigame succeeds
        public void OnFishingSuccess()
        {
            state = "ready";
            Destroy(spawnedRod);
        }

        // Called when minigame fails
        public void OnFishingFailure()
        {
            state = "cast";
        }

        public Card AttachedCard { get; set; }
    }
}
