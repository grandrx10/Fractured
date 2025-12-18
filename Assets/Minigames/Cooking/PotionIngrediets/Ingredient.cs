using Minigames.Cooking.CookingStuff;
using UnityEngine;

namespace Minigames.Cooking.PotionIngrediets
{
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "Cooking/Ingredient")]
    public class Ingredient : ScriptableObject
    {
        [Header("Ingredient Settings")]
        [Tooltip("Name the held object must have to be valid.")]
        public string ingredientName;

        [Tooltip("Should the item be rinsed to be valid?")]
        public bool requireRinsed = false;

        [Tooltip("Should the item be enchantable to be valid?")]
        public bool requireEnchanted = false;

        [Tooltip("Should the item be sealed to be valid?")]
        public bool requireSealed = false;

        [Header("Visual")]
        [Tooltip("Image representing this ingredient.")]
        public Sprite ingredientImage;
        public float duration;

        /// <summary>
        /// Checks if the held item meets the requirements for this ingredient.
        /// </summary>
        /// <param name="heldItem">The GameObject held by the player</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool CheckValid(GameObject heldItem)
        {
            if (heldItem == null) return false;

            // Check name
            if (!heldItem.name.Contains(ingredientName))
            {
                Debug.LogWarning($"Held item does not match ingredient name: {ingredientName}");
                return false;
            }

            // Check rinsed if required
            if (requireRinsed)
            {
                Rinsable rinsable = heldItem.GetComponent<Rinsable>();
                if (rinsable == null || !rinsable.rinsed)
                {
                    Debug.LogWarning($"Held item {heldItem.name} is not rinsed.");
                    return false;
                }
            }

            // Check enchanted if required
            if (requireEnchanted)
            {
                Enchantable enchantable = heldItem.GetComponent<Enchantable>();
                if (enchantable == null || !enchantable.isEnchanted)
                {
                    Debug.LogWarning($"Held item {heldItem.name} is not enchanted.");
                    return false;
                }
            }

            // Check sealed if required
            if (requireSealed)
            {
                Sealable sealable = heldItem.GetComponent<Sealable>();
                if (sealable == null || !sealable.isSealed)
                {
                    Debug.LogWarning($"Held item {heldItem.name} is not sealed.");
                    return false;
                }
            }

            return true;
        }
    }
}
