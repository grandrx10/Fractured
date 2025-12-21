using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cards.Core.Util
{
    public class CardCollector: MonoBehaviour
    {
        private void Awake()
        {
            List<Card> c = GetComponentsInChildren<Card>().ToList();
            Agent attached = GetComponent<Agent>();
            foreach (Card card in c)
            {
                attached.AddCard(card);
            }
            Destroy(this);
        }
    }
}