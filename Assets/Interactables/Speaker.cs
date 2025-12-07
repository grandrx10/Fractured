using UnityEngine;

public class Speaker : Interactable
{
    [Header("Conversation to Trigger")]
    public string conversationName;

    public override void Interact(GameObject player)
    {
        if (!string.IsNullOrEmpty(conversationName))
        {
            DialogueManager.Instance.StartConversation(conversationName);
        }
    }
}
