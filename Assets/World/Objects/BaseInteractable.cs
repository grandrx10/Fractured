using System;
using Characters.Interactables;
using UnityEngine;
using UnityEngine.Events;

namespace World.Objects
{
    public class BaseInteractable : Interactable
    {
        // this, Player, Init
        public UnityEvent<BaseInteractable, GameObject> onInteract;

        public override void Interact(GameObject player)
        {
            base.Interact(player);
            onInteract.Invoke(this, player);
        }
    }
}