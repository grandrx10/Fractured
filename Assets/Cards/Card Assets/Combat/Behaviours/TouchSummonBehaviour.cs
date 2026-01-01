using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;
using Game.Health;

namespace Cards.Core.Behaviors
{
    [CreateAssetMenu(fileName = "TouchSummon", menuName = "Behaviors/TouchSummon")]
    public class TouchSummonBehaviour : Behavior, IBehaviorHitListener
    {
        [Header("Prefab to Summon")]
        public GameObject prefabToSummon;

        public void Hit(OpenWorldEnv env, Agent agent, GameObject target)
        {   
            Debug.Log("TOUCH!");
            if (AttachedCard == null)
            {
                Debug.LogWarning("TouchSummonBehaviour: AttachedCard is null!");
                return;
            }

            if (prefabToSummon != null)
            {
                // Spawn the prefab at the card's position
                GameObject spawned = GameObject.Instantiate(prefabToSummon, AttachedCard.transform.position, Quaternion.identity);

                // Check if the spawned prefab has a Damaging component
                Damaging damagingComponent = spawned.GetComponent<Damaging>();
                if (damagingComponent != null)
                {
                    damagingComponent.card = AttachedCard;
                }
            }
            else
            {
                Debug.LogWarning("TouchSummonBehaviour: prefabToSummon is not assigned!");
            }

            // Destroy the card
            GameObject.Destroy(AttachedCard.gameObject);
        }
    }
}
