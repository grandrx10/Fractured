using System;
using UnityEngine;
using UnityEngine.Events;

namespace World.Objects
{
    public class BaseInteractable : Interactable
    {
        // this, Player, Init
        public UnityEvent<BaseInteractable, GameObject, bool> onInteract;

        private void Start()
        {
            onInteract.Invoke(this, null, true);
        }

        public override void Interact(GameObject player)
        {
            onInteract.Invoke(this, player, false);
        }
    }
}