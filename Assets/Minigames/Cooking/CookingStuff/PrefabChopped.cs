using UnityEngine;

namespace Minigames.Cooking.CookingStuff
{
    public class GumprootChoppable : Choppable
    {
        [Header("Chop Settings")]
        [Tooltip("Prefab to spawn when chopped")]
        public GameObject spawnPrefab;

        public override void Chop()
        {
            base.Chop(); // Call base if there is shared behavior

            if (spawnPrefab != null)
            {
                Instantiate(
                    spawnPrefab,
                    transform.position,
                    Quaternion.identity
                );
            }
            else
            {
                Debug.LogWarning("Spawn prefab not assigned for " + name);
            }

            // Optionally, destroy the original object after chopping
            Destroy(gameObject);
        }
    }
}
