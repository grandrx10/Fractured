using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(Rigidbody))]
    public class TorqueOnStart : MonoBehaviour
    {
        public float torqueMagnitude = 10f;
        public ForceMode forceMode = ForceMode.Impulse;

        void Start()
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            // Use the Rigidbody's orientation (important if interpolation / physics rotation)
            Vector3 forward = rb.linearVelocity.normalized;
            Vector3 up = rb.transform.up;

            // Wheel rotation axis
            Vector3 wheelAxis = Vector3.Cross(forward, up).normalized;
            // (this is equivalent to transform.right)

            rb.AddTorque(wheelAxis * torqueMagnitude, forceMode);
        }
    }
}