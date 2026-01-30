using UnityEngine;
using System.Collections.Generic;
using Utils;

namespace Cards.Card_Assets.RPS.Behaviors
{
    [RequireComponent(typeof(Collider))]
    public class StableCutter : MonoBehaviour
    {
        [Header("Normal")]
        public Vector3 normal = Vector3.up;
        public bool useLocalNormal = true;

        [Header("Rotation")]
        public bool randomRotation = false;

        [Header("Behavior")]
        public bool destroyAfterCut = false; // NEW FLAG

        private HashSet<GameObject> cutObjects = new HashSet<GameObject>();
        private Vector3 cutNormal;

        void Start()
        {
            ComputeNormal();
            
            // Ensure the collider is set as a trigger
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
            else
            {
                Debug.LogWarning("StableCutter requires a Collider component to detect collisions!");
            }
        }

        void ComputeNormal()
        {
            cutNormal = useLocalNormal
                ? transform.TransformDirection(normal).normalized
                : normal.normalized;

            // Optional random spin around the normal
            if (randomRotation)
            {
                float angle = Random.Range(0f, 360f);
                Quaternion randomRot = Quaternion.AngleAxis(angle, cutNormal);
                cutNormal = randomRot * cutNormal;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            var main = PhysicsHelper.MainObj(other);
            
            // Check if we've already cut this object
            if (cutObjects.Contains(main))
            {
                return;
            }

            // Try to cut the object
            var cuttable = main.GetComponent<ICuttable>();
            if (cuttable != null)
            {
                cuttable.Cut(cutNormal, transform.position);
                cutObjects.Add(main);

                if (destroyAfterCut)
                    Destroy(gameObject);

                return;
            }

            // Fallback to Slice component
            var slice = main.GetComponent<Slice>();
            if (slice != null)
            {
                slice.ComputeSlice(cutNormal, transform.position);
                cutObjects.Add(main);

                if (destroyAfterCut)
                    Destroy(gameObject);
            }
        }

        // Optional: Clear the cut history if needed
        public void ResetCutHistory()
        {
            cutObjects.Clear();
        }

        void OnDrawGizmosSelected()
        {
            Vector3 n = useLocalNormal
                ? transform.TransformDirection(normal).normalized
                : normal.normalized;

            // Draw normal direction
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                transform.position,
                transform.position + n * 2f
            );

            // Draw collider bounds if available
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}
