using System;
using Cards.Core;
using UnityEngine;

namespace Cards.PhysicalProperties
{
    public class PhysicalCard: PhysicalCardObject
    {
        public Card card;
        private Rigidbody _rb;
        public float lifetime;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

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