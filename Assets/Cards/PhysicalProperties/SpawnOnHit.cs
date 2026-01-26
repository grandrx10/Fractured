using UnityEngine;
using Utils;
using Cards.Core;

namespace Cards.PhysicalProperties
{
    public class SpawnOnHit : MonoBehaviour
    {
        private Rigidbody _rb;
        public float velocityFactor;
        public float delay;
        public float velOffset, normOffset;
        public bool atHitPoint;
        public GameObject prefab;
        
        private void OnEnable()
        {
            var pco = GetComponent<PhysicalObject>();
            if (!pco) Debug.LogError("expected PhysicalCardObject");

            pco.OnHit += state =>
            {
                var contact = state.Other.GetContact(0);
                
                // Get the card reference BEFORE the delay
                Card cardToTransfer = null;
                var myDamaging = GetComponent<Damaging>();
                if (myDamaging != null)
                {
                    cardToTransfer = myDamaging.card;
                }
                
                Delay.Call(delay, () =>
                {
                    var pos = atHitPoint? contact.point : state.Position;
                    pos += velOffset * state.Direction;
                    pos += normOffset * contact.normal;
                    var p = Instantiate(prefab, pos, Quaternion.LookRotation(state.Velocity));
                    
                    if (velocityFactor != 0)
                    {
                        var rb = p.GetComponent<Rigidbody>();
                        rb.linearVelocity = state.Direction * state.Speed * velocityFactor;
                    }
                    
                    // Transfer card reference to spawned prefab's Damaging component
                    var spawnedDamaging = p.GetComponent<Damaging>();
                    if (spawnedDamaging != null && cardToTransfer != null)
                    {
                        spawnedDamaging.card = cardToTransfer;
                    }
                });
                Destroy(this);
            };
        }
    }
}