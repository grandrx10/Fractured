using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;

namespace Cards.Card_Assets.Combat.Behaviours
{
    [CreateAssetMenu(fileName = "TouchSummon", menuName = "Behaviors/TouchSummon")]
    public class TouchSummonBehaviour : Behavior, IBehaviorHitListener
    {
        [Header("Prefab to Summon")]
        public GameObject prefabToSummon;

        public void Hit(Card card, OpenWorldEnv env, GameObject target)
        {   
            Debug.Log("TOUCH!");
            if (card == null)
            {
                Debug.LogWarning("TouchSummonBehaviour: AttachedCard is null!");
                return;
            }

            if (prefabToSummon != null)
            {
                // Spawn the prefab at the card's position
                GameObject spawned = GameObject.Instantiate(prefabToSummon, card.transform.position, Quaternion.identity);

                // Check if the spawned prefab has a Damaging component
                Damaging damagingComponent = spawned.GetComponent<Damaging>();
                if (damagingComponent != null)
                {
                    damagingComponent.card = card;
                }
            }
            else
            {
                Debug.LogWarning("TouchSummonBehaviour: prefabToSummon is not assigned!");
            }

            // Destroy the card
            GameObject.Destroy(card.gameObject);
        }
    }
}
