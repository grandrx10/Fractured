using UnityEngine;

namespace Characters.Dialogue
{
    public class OpenDoorEvent : DialogueEvent
    {
        [Header("Door")]
        public Door door;

        public override void Execute()
        {
            if (door == null)
            {
                Debug.LogWarning("OpenDoorEvent has no Door assigned.");
                return;
            }

            door.Open();
        }
    }
}
