using UnityEngine;
using Utils;

namespace Cards.PhysicalProperties
{
    public class DestroyOnHit : MonoBehaviour
    {
        public float delay;
        private void Awake()
        {
            var pco = GetComponent<PhysicalObject>();
            if (!pco) Debug.LogError("expected PhysicalCardObject");

            pco.OnHit += _ =>
            {
                GameObject go = gameObject;
                Delay.Call(delay, () =>
                {
                    Destroy(go);
                });
                Destroy(this);
            };
        }
    }
}