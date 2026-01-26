using Characters.Dialogue;
using UnityEngine;
using Game;

namespace Characters.Interactables
{
    public class Speaker : Interactable
    {
        [Header("Conversation to Trigger")]
        [SerializeField] private string defaultConversationName;

        public string conversationName
        {
            get
            {
                if (GlobalState.instance != null &&
                    GlobalState.instance.strs.TryGetValue(GetPersistentKey(), out var savedName))
                {
                    return savedName;
                }

                return defaultConversationName;
            }
            set
            {
                if (GlobalState.instance != null)
                    GlobalState.instance.SetStr(GetPersistentKey(), value);
                else
                    defaultConversationName = value;

                RefreshCanInteract();
            }
        }

        private void Start()
        {
            RefreshCanInteract();
        }

        public override void Interact(GameObject player)
        {
            if (!string.IsNullOrEmpty(conversationName))
            {
                DialogueManager.Instance.StartConversation(conversationName);
            }
        }

        private void RefreshCanInteract()
        {
            canInteract = !string.IsNullOrEmpty(conversationName);
        }

        private string GetPersistentKey()
        {
            return $"SpeakerConversation_{gameObject.scene.name}_{gameObject.name}";
        }
    }
}
