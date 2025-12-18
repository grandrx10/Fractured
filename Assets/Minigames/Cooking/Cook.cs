using Minigames.Cooking.CookingStuff;
using UnityEngine;

namespace Minigames.Cooking
{
    public class Cook : MonoBehaviour
    {
        [Header("Hand Settings")]
        [Tooltip("Transform representing where the cook holds items")]
        public Transform handTransform;

        [Header("Debug")]
        [Tooltip("Currently held object")]
        public Cookable heldObject;

        /// <summary>
        /// Call this to interact with a cookable object
        /// </summary>
        public void InteractWithCookable(Cookable cookable)
        {
            if (cookable == null) return;

            // If holding an object, drop it first
            if (heldObject != null)
            {
                DropCurrentObject();
            }

            // Pick up the new cookable
            heldObject = cookable;

            // Parent to hand
            heldObject.transform.SetParent(handTransform);

            // Move to hand position
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;

            // Make rigidbody kinematic
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }

        /// <summary>
        /// Drops the currently held object
        /// </summary>
        public void DropCurrentObject()
        {
            if (heldObject == null) return;

            // Unparent
            heldObject.transform.SetParent(null);

            // Make rigidbody non-kinematic
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            heldObject = null;
        }
    }
}
