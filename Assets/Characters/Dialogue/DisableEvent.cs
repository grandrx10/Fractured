using UnityEngine;

namespace Characters.Dialogue
{
    public class DisableEvent : DialogueEvent
    {
        [Header("Objects To Disable")]
        public GameObject[] objectsToDisable;

        public override void Execute()
        {
            if (objectsToDisable == null || objectsToDisable.Length == 0)
                return;

            foreach (GameObject obj in objectsToDisable)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}
