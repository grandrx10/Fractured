using System.Collections;
using System.Collections.Generic;
using Cards.Card_Assets.General_Behaviors;
using Cards.Core;
using UnityEngine;
using Game; // for GlobalState

namespace Characters.Dialogue
{
    public class IntroQuestEvent : DialogueEvent
    {
        public string questName;
        [TextArea] public string questDescription;
        private void Start()
        {
            // Check persistent state
            if (GlobalState.instance.HasEvent("IntroQuestsGiven"))
            {
                return;
            }

            GlobalState.instance.AddEvent("IntroQuestsGiven");
            GlobalState.instance.AddQuest("fake_quest_1", "Relax on the beach!", true);
            GlobalState.instance.AddQuest("fake_quest_2", "Do something cool later", false);
            GlobalState.instance.AddQuest("fake_quest_3", questDescription, false);
            GlobalState.instance.AddQuest("fake_quest_4", questDescription, false);
        }
    }
}
