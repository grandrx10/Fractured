using System.Collections;
using System.Collections.Generic;
using Cards.Card_Assets.General_Behaviors;
using Cards.Core;
using UnityEngine;
using Game; // for GlobalState

namespace Characters.Dialogue
{
    public class CompleteQuestEvent : DialogueEvent
    {
        public List<string> requiredItems;
        public string keyName;
        public override void Execute()
        {
            var player = GlobalWorldManager.Instance
                .CurrentEnvironment
                .player;
            var pCards = player.GetAllCards();
            
            Dictionary<string, int> requiredCounts = new Dictionary<string, int>();
            foreach (var req in requiredItems)
            {
                if (!requiredCounts.TryAdd(req, 1))
                    requiredCounts[req]++;
            }
            
            List<Card> matchedCards = new List<Card>();
            
            foreach (var card in pCards)
            {
                if (!card.TryGetBehavior(out QuestItemBehavior q)) continue;
                if (requiredCounts.TryGetValue(q.itemName, out int remaining) && remaining > 0)
                {
                    matchedCards.Add(card);
                    requiredCounts[q.itemName] = remaining - 1;
                }
            }

            bool hasAll = true;
            foreach (var kvp in requiredCounts)
            {
                if (kvp.Value > 0)
                {
                    hasAll = false;
                    Debug.Log($"Failed to find: {kvp.Value} of {kvp.Key}");
                    break;
                }
            }

            if (hasAll)
            {
                GlobalState.instance.AddEvent(keyName);
            }
        }
    }
}
