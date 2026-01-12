using UnityEngine;
using Utils;
using Cards.PhysicalProperties;
using Cards.Core;

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
            if (!pco) Debug.LogError("Expected PhysicalObject on this GameObject!");

            pco.OnMove += _ =>
            {
                Delay.Call(this, delay, () =>
                {
                    var crb = GetComponent<Rigidbody>();
                    var rbInstance = Instantiate(prefab, transform.position, transform.rotation);
                    rbInstance.linearVelocity = crb.linearVelocity;

                    // If the new object has a Damaging component, link the card
                    var damaging = rbInstance.GetComponent<Damaging>();
                    if (damaging != null && pco.card != null)
                    {
                        damaging.card = pco.card;
                    }
                    
                    var newpco = rbInstance.GetComponent<PhysicalObject>();
                    if (newpco != null)
                    {
                        newpco.card = pco.card;
                    }

                    if (destroy) Destroy(gameObject);
                });
            };
        }
    }
}
