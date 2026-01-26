using Cards.Environments;
using Characters;
using UnityEngine;
using Minigames.Cooking.Stations;

namespace Minigames.Cooking
{
    public class ExplodeManager : MonoBehaviour
    {
        public static ExplodeManager Instance { get; private set; }

        [Header("Explosion Prefab")]
        public GameObject explosionPrefab;
        public PotStation pot;

        private bool hasExploded = false; // prevent multiple explosions

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Explodes at a transform and triggers the associated PotStation fail once
        /// </summary>
        /// <param name="target">Position of explosion</param>
        public void Explode(Transform target)
        {
            if (hasExploded) return;
            hasExploded = true;

            if (target == null || explosionPrefab == null) return;

            Instantiate(explosionPrefab, target.position, target.rotation);
            Debug.Log($"Explosion spawned at {target.position}");

            // Automatically find the PotStation at this position and trigger fail
            if (pot == null)
            {
                // If the PotStation is on the parent or nearby
                pot = target.GetComponentInParent<PotStation>();
            }
            if (pot != null)
            {
                pot.TriggerFail();
            }

            // Clear the player's hand
            Cook cook = OpenWorldEnv.Current.player.GetComponent<Cook>();
            if (cook != null && cook.heldObject != null)
            {
                cook.heldObject.transform.parent = null;
                Destroy(cook.heldObject.gameObject);
                cook.heldObject = null;
                Debug.Log("Player's hand cleared due to explosion.");
            }
        }

        /// <summary>
        /// Reset explosion to allow a new one
        /// </summary>
        public void ResetExplosion()
        {
            hasExploded = false;
        }
    }
}
