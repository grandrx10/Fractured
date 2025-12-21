using Minigames.Cooking.CookingStuff;
using UnityEngine;

namespace Minigames.Cooking.Stations
{
    public class ChoppingStation : Station
    {

        // // Override the criteria check to only allow Choppable objects
        // protected override bool CheckCriteria(Cookable heldObject)
        // {
        //     return heldObject.GetComponent<Choppable>() != null;
        // }

        // Override the interaction behavior
        public override void Interact(GameObject player)
        {
            // Get the Cook component from the singleton
            Cook cook = Characters.PlayerSingleton.Instance.GetComponent<Cook>();
            if (cook == null) return;

            Cookable heldObject = cook.heldObject;
            if (heldObject == null) return;

            if (heldObject.GetComponent<Choppable>() == null)
            {
                Explode();
            }

            // Double-check criteria
            if (!CheckCriteria(heldObject)) return;

            // Move the object to the chopping point
            if (stationPoint != null)
            {
                heldObject.transform.position = stationPoint.position;
                heldObject.transform.rotation = stationPoint.rotation;
            }

            // Make Rigidbody kinematic and disable collisions
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }

            // Trigger chop effect
            Choppable choppable = heldObject.GetComponent<Choppable>();
            if (choppable != null)
            {
                choppable.Chop();
            }

            // Remove from cook's hand
            cook.heldObject = null;
            heldObject.transform.parent = null;
        }
    }
}
