using System;
using Cards;
using Cards.Core;
using UnityEngine;
using World.Objects;

public class Chest : MonoBehaviour
{
    public Animator animator;
    public CardData card;
    public string chestId;
    private void Init(BaseInteractable I)
    {
        if (chestId != "" && GlobalState.instance.HasEvent($"CHEST_{chestId}_OPEN"))
        {
            animator.Play("Open", -1, 1);
            I.canInteract = false;
        }
    }

    public void Interact(BaseInteractable I, GameObject player, bool init)
    {
        if (init)
        {
            Init(I);
            return;
        }
        animator.Play("Open");
        I.canInteract = false;
        
        GameObject cardGo = new GameObject("Card");
        Card c = cardGo.AddComponent<Card>();
        c.AssignData(card);
        player.GetComponentInChildren<PlayerAgent>().AddCard(c);
        
        if (chestId != "")
        {
            GlobalState.instance.AddEvent($"CHEST_{chestId}_OPEN");
        }
    }
}
