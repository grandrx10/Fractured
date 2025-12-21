using UnityEngine;
using Utils;

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
                });
                Destroy(this);
            };
        }
    }
}