using UnityEngine;

public class Speaker : Interactable
{
    public override void Interact(GameObject player)
    {
        Debug.Log("I've been interacted with");

        Follower follower = gameObject.GetComponent<Follower>();
        if (follower != null)
        {
            follower.SetFollowable(player.GetComponent<Followable>());
            canInteract = false;
        }
    }
}
