using Characters;
using Minigames.Cooking.CookingStuff;
using UnityEngine;

namespace Minigames.Cooking.Stations
{
    public class SealerStation : Station
    {
        public override void Interact(GameObject player)
        {
            // Get the Cook component from the singleton
            Cook cook = PlayerSingleton.Instance.GetComponent<Cook>();
            if (cook == null) return;

            Cookable heldObject = cook.heldObject;
            if (heldObject == null) return;

            Sealable sealable = heldObject.GetComponent<Sealable>();
            if (sealable == null)
            {
                // Not sealable → explode
                Explode();
                return;
            }

            // Seal the object
            sealable.isSealed = true;
            Debug.Log($"Sealed {heldObject.name}");

            // Move object to station point (same behavior as RinsingStation)
            if (stationPoint != null)
            {
                heldObject.transform.position = stationPoint.position;
                heldObject.transform.rotation = stationPoint.rotation;
            }

            // Unparent from player
            heldObject.transform.parent = null;

            // Remove from cook's hand
            cook.heldObject = null;
            heldObject.canInteract = true;
            heldObject.OnDropped();
        }
    }
}
