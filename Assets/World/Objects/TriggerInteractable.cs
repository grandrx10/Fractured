using System;
using Characters.Interactables;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace World.Objects
{
    public class TriggerInteractable : MonoBehaviour
    {
        // this, Player, Init
        public UnityEvent onInteract;

        private void OnTriggerEnter(Collider other)
        {
            if (PhysicsHelper.MainObj(other).tag == "Player")
            {
                onInteract.Invoke();
            }
        }
    }
}