using System;
using Cards.Core;
using UnityEngine;

namespace Cards.Visual
{
    public class InventoryCardCollector : MonoBehaviour
    {
        public Agent agent;

        private void Start()
        {
            foreach (var card in GetComponentsInChildren<Card>())
            {
                agent.AddCard(card);
            }
        }
    }
}