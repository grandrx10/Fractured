using Characters.Dialogue;
using UnityEngine;
using Game; // For GlobalState

namespace Characters.Interactables
{
    public class Speaker : Interactable
    {
        [Header("Conversation to Trigger")]
        [SerializeField] private string defaultConversationName; // Original default conversation

        // This property gets/sets the conversation persistently
        public string conversationName
        {
            get
            {
                if (GlobalState.instance != null && GlobalState.instance.strs.TryGetValue(GetPersistentKey(), out var savedName))
                    return savedName;

                return defaultConversationName;
            }
            set
            {
                if (GlobalState.instance != null)
                    GlobalState.instance.SetStr(GetPersistentKey(), value);
                else
                    defaultConversationName = value;
            }
        }

        public override void Interact(GameObject player)
        {
            if (!string.IsNullOrEmpty(conversationName))
            {
                DialogueManager.Instance.StartConversation(conversationName);
            }
        }

        private string GetPersistentKey()
        {
            // Use unique key per speaker based on object name + scene
            return $"SpeakerConversation_{gameObject.scene.name}_{gameObject.name}";
        }
    }
}
