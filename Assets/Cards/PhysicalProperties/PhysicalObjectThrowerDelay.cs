using UnityEngine;
using Utils;

namespace Cards.PhysicalProperties
{
    public class PhysicalObjectThrowerDelay : MonoBehaviour
    {
        public Rigidbody prefab;
        public float delay;
        public bool destroy = true;
        private void Awake()
        {
            var pco = GetComponent<PhysicalObject>();
            if (!pco) Debug.LogError("expected PhysicalCardObject");

            pco.OnMove += _ =>
            {
                Delay.Call(this, delay, () =>
                {
                    var crb = GetComponent<Rigidbody>();
                    var rb = Instantiate(prefab, transform.position, transform.rotation);
                    rb.linearVelocity = crb.linearVelocity;
                    if (destroy) Destroy(gameObject);
                });
            };
        }
    }
}