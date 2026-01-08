using System.Collections;
using UnityEngine;
using Game; // for GlobalState

namespace Characters.Dialogue
{
    public class TriggerDialogueEvent : DialogueEvent
    {
        [Header("Dialogue")]
        [Tooltip("Conversation name to start")]
        public string conversationName;

        [Header("Trigger Settings")]
        public bool triggerOnAwake = false;
        public bool triggerOnlyOnce = true;
        public bool triggerOnceEver = false; // NEW: persists across scene loads

        private bool hasTriggered = false;

        private string globalEventKey => $"DialogueEvent_{conversationName}";

        private void Awake()
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
            if ((hasTriggered && triggerOnlyOnce) || (triggerOnceEver && GlobalState.instance.HasEvent(globalEventKey)))
                return;

            if (DialogueManager.Instance == null)
            {
                Debug.LogWarning("DialogueManager not found.");
                return;
            }

            DialogueManager.Instance.StartConversation(conversationName);
            hasTriggered = true;

            if (triggerOnceEver)
            {
                GlobalState.instance.AddEvent(globalEventKey);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((hasTriggered && triggerOnlyOnce) || (triggerOnceEver && GlobalState.instance.HasEvent(globalEventKey)))
                return;

            if (other.CompareTag("Player"))
            {
                TryTrigger();
            }
        }
    }
}
