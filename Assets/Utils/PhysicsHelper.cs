using UnityEngine;

namespace Utils
{
    public static class PhysicsHelper
    {
        public static GameObject MainObj(Collider collider)
        {
            if (!collider) return null;
            return collider.attachedRigidbody? collider.attachedRigidbody.gameObject : collider.gameObject;
        }
        
        public static bool IsInMask(int layer, LayerMask mask)
        {
            return (mask.value & (1 << layer)) != 0;
        }
    }
}