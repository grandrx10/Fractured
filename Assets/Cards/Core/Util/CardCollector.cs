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
            attached.AddCards(c);
            Destroy(this);
        }
    }
}