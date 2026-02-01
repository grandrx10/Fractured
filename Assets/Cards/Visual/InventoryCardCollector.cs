using System;
using Cards.Core;
using UnityEngine;

namespace Cards.Visual
{
    public class InventoryCardCollector : MonoBehaviour
    {
        public Agent agent;
        public bool hand;
        private void Start()
        {
#if UNITY_EDITOR
            foreach (var card in GetComponentsInChildren<Card>())
            {
                agent.GiveCard(card, hand);
            }
#endif
        }
    }
}