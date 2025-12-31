using UnityEngine;

namespace Characters.Interactables
{
    public class Lever : Interactable
    {
        [Header("Lever Settings")]
        [Tooltip("Door controlled by this lever")]
        public LeverDoor targetDoor;

        [Tooltip("Rotation when lever is ON")]
        public Vector3 onRotation = new Vector3(-45f, 0f, 0f);

        [Tooltip("Rotation when lever is OFF")]
        public Vector3 offRotation = new Vector3(45f, 0f, 0f);

        private bool isOn = false;

        public override void Interact(GameObject player)
        {
            if (!canInteract || targetDoor == null)
                return;

            isOn = !isOn;

            targetDoor.Toggle();
        }
    }
}
