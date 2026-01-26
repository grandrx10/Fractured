using Cards.Environments;
using Minigames.Cooking.CookingStuff;
using UnityEngine;

namespace Minigames.Cooking.Stations
{
    public class GarbageStation : Station
    {
        public override void Interact(GameObject player)
        {
            Cook cook = OpenWorldEnv.Current
                ?.PlayerTransform
                ?.GetComponent<Cook>();

            if (cook == null)
                return;

            Cookable heldObject = cook.heldObject;
            if (heldObject == null)
                return;

            // Destroy the held object
            Destroy(heldObject.gameObject);

            // Clear the cook's hand
            cook.heldObject = null;
        }

        // Garbage accepts anything
        protected override bool CheckCriteria(Cookable heldObject)
        {
            return heldObject != null;
        }
    }
}
