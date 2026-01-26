using System.Collections;
using System.Collections.Generic;
using Cards.Card_Assets.General_Behaviors;
using Cards.Core;
using UnityEngine;
using Game; // for GlobalState

namespace Characters.Dialogue
{
    public class StartQuestEvent : DialogueEvent
    {
        public string questName;
        [TextArea] public string questDescription;
        public override void Execute()
        {
            GlobalState.instance.AddQuest(questName, questDescription, false);
        }
    }
}
