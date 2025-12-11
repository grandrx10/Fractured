using System;
using Cards.Core;
using UnityEngine;

namespace Cards.PhysicalProperties
{
    public class PhysicalCard: PhysicalObject
    {
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