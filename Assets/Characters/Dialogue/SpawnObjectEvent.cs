using System.Collections;
using UnityEngine;
using Game; // for GlobalState

namespace Characters.Dialogue
{
    [RequireComponent(typeof(PersistentID))]
    public class SpawnObjectEvent : DialogueEvent
    {
        public Transform location;
        public GameObject prefab;
        [Header("Trigger Settings")]
        public bool triggerOnAwake = false;
        public bool triggerOnlyOnce = true;
        public bool triggerOnceEver = false;

        private bool hasTriggered = false;
        private string globalEventKey => $"SpawnEvent_{GetComponent<PersistentID>().ID}";

        private void Start()
        {
            // Check persistent state
            if (triggerOnceEver && GlobalState.instance.HasEvent(globalEventKey))
            {
                hasTriggered = true;
                return;
            }

            if (triggerOnAwake)
            {
                // Start coroutine to wait until DialogueManager exists
                StartCoroutine(WaitAndTrigger());
            }
        }

        public override void Execute()
        {
            TryTrigger();
        }

        private IEnumerator WaitAndTrigger()
        {
            // Wait until DialogueManager.Instance is not null
            while (DialogueManager.Instance == null)
                yield return null;

            TryTrigger();
        }

        public void TryTrigger()
        {
            Instantiate(prefab, location.position, location.rotation);
            GlobalState.instance.AddEvent(globalEventKey);
        }
    }
}
