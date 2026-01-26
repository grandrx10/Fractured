using Cards;
using Cards.Core;
using Characters.Player;
using Game;
using UnityEngine;
using Characters.Dialogue;
using Cards.Environments;

namespace World.Objects
{
    [System.Serializable]
    public class GiveCardEvent : DialogueEvent
    {
        [Header("Card to Give")]
        public CardData card;

        [Header("Give to hand")]
        public bool giveToHand = false;

        public override void Execute()
        {
            if (OpenWorldEnv.Current == null || OpenWorldEnv.Current.player == null)
            {
                Debug.LogError("GiveCardEvent: No player found in OpenWorldEnv!");
                return;
            }

            PlayerAgent player = OpenWorldEnv.Current.player;

            // Create card GameObject and assign data
            GameObject cardGo = new GameObject("Card");
            Card c = cardGo.AddComponent<Card>();
            c.AssignData(card);

            if (giveToHand)
            {
                // Put card directly in the main hand
                player.GiveCard(c, true);
                Debug.Log($"GiveCardEvent: Gave card '{card.name}' to player's hand.");
            }
            else
            {
                // Add card to inventory
                player.GiveCard(c);
                Debug.Log($"GiveCardEvent: Gave card '{card.name}' to inventory.");
            }
        }
    }
}
