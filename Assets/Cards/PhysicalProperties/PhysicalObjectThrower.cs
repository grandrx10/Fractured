using UnityEngine;

namespace Cards.PhysicalProperties
{
    public class PhysicalObjectThrower : MonoBehaviour
    {
        public Rigidbody prefab;
        
        private void Awake()
        {
            var pco = GetComponent<PhysicalObject>();
            if (!pco) Debug.LogError("expected PhysicalCardObject");

            pco.OnMove += state =>
            {
                var rb = Instantiate(prefab, state.StartPosition, Quaternion.LookRotation(state.StartDirection));
                rb.linearVelocity = state.StartDirection * state.Speed;
                Destroy(gameObject);
            };
        }
        
    }
}