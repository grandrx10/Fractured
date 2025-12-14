using System.Collections;
using UnityEngine;

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

        private bool hasTriggered = false;

        private void Awake()
        {
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
            if (hasTriggered && triggerOnlyOnce)
                return;

            if (DialogueManager.Instance == null)
            {
                Debug.LogWarning("DialogueManager not found.");
                return;
            }

            DialogueManager.Instance.StartConversation(conversationName);
            hasTriggered = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered && triggerOnlyOnce)
                return;

            if (other.CompareTag("Player"))
            {
                TryTrigger();
            }
        }
    }
}
