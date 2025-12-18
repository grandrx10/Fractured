using UnityEngine;

namespace Minigames.Cooking.CookingStuff
{
    public class Enchantable : MonoBehaviour
    {
        [Header("Enchant Settings")]
        public GameObject transformedPrefab;   // Prefab to replace with
        public float enchantDuration = 3f;     // Time to wait before transformation
        public float expireTime = 5f;          // Time after transform before exploding if not picked up
        [HideInInspector] public bool isEnchanted = false; // Flag for enchanted objects
    }
}