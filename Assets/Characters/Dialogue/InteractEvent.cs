using Cards;
using Cards.Core;
using Characters.Player;
using Game;
using UnityEngine;
using Characters.Dialogue;
using Cards.Environments;
using Characters.Interactables;

namespace World.Objects
{
    public class InteractEvent : DialogueEvent
    {
        public Interactable interactable;

        public override void Execute()
        {
            interactable.Interact(gameObject);
        }
    }
}