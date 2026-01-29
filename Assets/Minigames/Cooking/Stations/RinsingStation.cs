using Cards.Environments;
using Characters;
using Minigames.Cooking.CookingStuff;
using UnityEngine;

namespace Minigames.Cooking.Stations
{
    public class RinsingStation : Station
    {

        public override void Interact(GameObject player)
        {
            base.Interact(player);
            // Get the Cook component from the singleton
            Cook cook = OpenWorldEnv.Current
                ?.PlayerTransform
                ?.GetComponent<Cook>();
            if (cook == null) return;

            Cookable heldObject = cook.heldObject;
            if (heldObject == null) return;

            Rinsable rinsable = heldObject.GetComponent<Rinsable>();

            if (rinsable == null)
            {
                // Not rinsable → explode
                Explode();
                return;
            }

            // Rinse the object
            rinsable.Rinse();
            Debug.Log($"Rinsed {heldObject.name}");

            // Optionally, you can keep it in the player's hand or move it to the station
            if (stationPoint != null)
            {
                heldObject.transform.position = stationPoint.position;
                heldObject.transform.rotation = stationPoint.rotation;
            }

            // Unparent from player
            heldObject.transform.parent = null;

            // Remove from cook's hand if desired
            cook.heldObject = null;
            heldObject.canInteract = true;
            heldObject.OnDropped();
        }
    }
}
