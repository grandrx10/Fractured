using System;
using Cards;
using Cards.Core;
using Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace World.Objects
{
    public class Chest : MonoBehaviour
    {
        public Animator animator;
        public CardData card;
        public PersistentID id;

        private void Awake()
        {
            GlobalWorldManager.RunOnNextLoad(e =>
            {
                if (id.ID != "" && GlobalState.instance.HasEvent($"CHEST_{id.ID}_OPEN"))
                {
                    animator.Play("Open", -1, 1);
                    GetComponent<BaseInteractable>().canInteract = false;
                }
            });
        }

        public void Interact(BaseInteractable I, GameObject player)
        {
            animator.Play("Open");
            I.canInteract = false;
        
            GameObject cardGo = new GameObject("Card");
            Card c = cardGo.AddComponent<Card>();
            c.AssignData(card);
            player.GetComponentInChildren<PlayerAgent>().GiveCard(c);
        
            if (id.ID != "")
            {
                GlobalState.instance.AddEvent($"CHEST_{id.ID}_OPEN");
            }
        }
    }
}
