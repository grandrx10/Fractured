using Characters.Interactables;
using UnityEngine;

namespace Minigames.Cooking.CookingStuff
{
    public class Cookable : Interactable
    {
        private Rigidbody rb;
        private Collider[] colliders;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            colliders = GetComponentsInChildren<Collider>();
        }

        public override void Interact(GameObject player)
        {
            base.Interact(player);
            Cook cook = player.GetComponent<Cook>();
            if (cook == null) return;

            // Swap / pick up the cookable via the cook
            Cookable previous = cook.heldObject;
            cook.InteractWithCookable(this);

            // Make this object kinematic and disable collisions
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }

            // If there was a previously held object, re-enable its colliders and Rigidbody
            if (previous != null && previous != this)
            {
                previous.OnDropped();
            }

            canInteract = false;
        }

        /// <summary>
        /// Call this when the object is dropped
        /// </summary>
        public void OnDropped()
        {
            if (rb != null)
                rb.isKinematic = false;

            foreach (Collider col in colliders)
                col.enabled = true;

            transform.SetParent(null);
            canInteract = true;
        }
    }
}
