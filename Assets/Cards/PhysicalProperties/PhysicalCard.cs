using System;
using Cards.Core;
using UnityEngine;

namespace Cards.PhysicalProperties
{
    public class PhysicalCard: PhysicalObject
    {
        
        public float lifetime;
        
        private void Update()
        {
            lifetime -= Time.deltaTime;
            if (0 > lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}