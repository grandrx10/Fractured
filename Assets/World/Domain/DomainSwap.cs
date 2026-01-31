using System;
using System.Collections.Generic;
using UnityEngine;
using World.Objects;

namespace World.Domain
{
    public class DomainSwap : MonoBehaviour
    {
        public DomainTrigger target;
        public List<Pairs> pairs;
        [Serializable]
        public struct Pairs
        {
            public string targetName;
            public string tagName;
        }

        private void Start()
        {
            foreach (Pairs pair in pairs)
            {
                if (GlobalWorldManager.Instance.TransitionTag.Contains(pair.tagName))
                {
                    target.domainPoint = pair.targetName;
                }
            }
        }
    }
}