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

            // Give card to player
            player.AddCard(c);

            Debug.Log($"GiveCardEvent: Gave card '{card.name}' to player.");
        }
    }
}
