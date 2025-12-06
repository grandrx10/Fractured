using UnityEngine;

public class Speaker : Interactable
{
    public Conversation conversation;

    public override void Interact(GameObject player)
    {
        if (conversation != null)
        {
            DialogueManager.Instance.StartConversation(conversation);
        }
    }
}
