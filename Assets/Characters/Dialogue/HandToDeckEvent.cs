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
    public class HandToDeckEvent : DialogueEvent
    {
        public override void Execute()
        {
            if (OpenWorldEnv.Current == null || OpenWorldEnv.Current.player == null)
            {
                Debug.LogError("GiveCardEvent: No player found in OpenWorldEnv!");
                return;
            }

            PlayerAgent player = OpenWorldEnv.Current.player;
            
        }
    }
}
