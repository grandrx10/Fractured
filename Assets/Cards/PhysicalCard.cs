using System;
using Cards.Core;
using UnityEngine;

namespace Cards
{
    public class PhysicalCard: MonoBehaviour
    {
        public Card card;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
    }
}