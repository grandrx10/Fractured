using Characters;
using UnityEngine;

namespace Minigames.Cooking
{
    public class ExplodeManager : MonoBehaviour
    {
        public static ExplodeManager Instance { get; private set; }

        [Header("Explosion Prefab")]
        public GameObject explosionPrefab; // Assign the explosion effect prefab in inspector

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
        /// Spawns the explosion prefab at the given transform and clears the player's held object.
        /// </summary>
        /// <param name="target">Transform where the explosion occurs</param>
        public void Explode(Transform target)
        {
            if (target == null || explosionPrefab == null) return;

            // Spawn explosion prefab
            Instantiate(explosionPrefab, target.position, target.rotation);
            Debug.Log($"Explosion spawned at {target.position}");

            // Clear the player's hand if holding something
            if (PlayerSingleton.Instance != null)
            {
                Cook cook = PlayerSingleton.Instance.GetComponent<Cook>();
                if (cook != null && cook.heldObject != null)
                {
                    // Unparent and destroy the held object
                    cook.heldObject.transform.parent = null;
                    Destroy(cook.heldObject.gameObject);
                    cook.heldObject = null;

                    Debug.Log("Player's hand cleared due to explosion.");
                }
            }
        }
    }
}
