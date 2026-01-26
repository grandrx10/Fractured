using System.Collections;
using UnityEngine;
using Game;
using World.Domain; // for GlobalState

namespace Characters.Dialogue
{
    public class DomainDialogueEvent : DialogueEvent, IDomainTriggerable
    {
        [Header("Dialogue")]
        [Tooltip("Conversation name to start")]
        public string conversationName;

        public override void Execute()
        {
            TryTrigger();
        }
        public void TryTrigger()
        {
            if (DialogueManager.Instance == null)
            {
                Debug.LogWarning("DialogueManager not found.");
                return;
            }

            DialogueManager.Instance.StartConversation(conversationName);
        }

        public void Trigger()
        {
            TryTrigger();
        }

        public void Trigger(Vector3 startPoint)
        {
            TryTrigger();
        }
    }
}
