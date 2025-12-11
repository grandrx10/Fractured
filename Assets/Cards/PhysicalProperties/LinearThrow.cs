using UnityEngine;

namespace Cards.PhysicalProperties
{
    public class LinearThrow : MonoBehaviour
    {
        private Rigidbody _rb;
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;

            var pco = GetComponent<PhysicalObject>();
            if (!pco) Debug.LogError("expected PhysicalCardObject");

            pco.OnMove += state =>
            {
                _rb.isKinematic = false;
                _rb.linearVelocity = state.StartDirection * state.Speed;
                Destroy(this);
            };
        }
    }
}