using Characters.Dialogue;
using UnityEngine;
using Game;

namespace Characters.Interactables
{
    public class Speaker : Interactable
    {
        [Header("Conversation to Trigger")]
        [SerializeField] private string defaultConversationName;

        // Static reference to the default speaker interact sound
        private static AudioClip defaultSpeakerSound;

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

        private void Awake()
        {
            // Load the default sound once
            if (defaultSpeakerSound == null)
            {
                defaultSpeakerSound = Resources.Load<AudioClip>("Audio/BaseInteract");
            }

            // Assign it as the interactSound if not already assigned
            if (interactSound == null)
            {
                interactSound = defaultSpeakerSound;
            }
        }

        private void Start()
        {
            RefreshCanInteract();
        }

        public override void Interact(GameObject player)
        {
            base.Interact(player); // will play default sound automatically

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
