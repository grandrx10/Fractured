using UnityEngine;
using Utils;

namespace Cards.Card_Assets.RPS.Behaviors
{
    public class Cutter : MonoBehaviour
    {
        [Header("Hitbox (World Space)")]
        public float size = 1f;               // half side length (world units)
        public float epsilonHeight = 0.01f;

        [Header("Normal")]
        public Vector3 normal = Vector3.up;   // arbitrary normal (world space)
        public bool useLocalNormal = true;

        [Header("Rotation")]
        public bool randomRotation = false;

        Quaternion cutRotation;

        void Start()
        {
            ComputeRotation();
            PerformCut();
        }

        void ComputeRotation()
        {
            Vector3 n = useLocalNormal
                ? transform.TransformDirection(normal).normalized
                : normal.normalized;

            // Build a stable rotation whose up axis = normal
            cutRotation = Quaternion.FromToRotation(Vector3.up, n);

            // Optional random spin around the normal
            if (randomRotation)
            {
                float angle = Random.Range(0f, 360f);
                cutRotation = Quaternion.AngleAxis(angle, n) * cutRotation;
            }
        }

        void PerformCut()
        {
            Vector3 halfExtents = new Vector3(size, epsilonHeight, size);

            Collider[] hits = Physics.OverlapBox(
                transform.position,
                halfExtents,
                cutRotation
            );
            foreach (var hit in hits)
            {
                var main = PhysicsHelper.MainObj(hit);
                var cuttable = main.GetComponent<ICuttable>();
                if (cuttable == null)
                {
                    var slice = main.GetComponent<Slice>();
                    if (slice != null) slice.ComputeSlice(GetNormal(), transform.position);
                    return;
                }
                
                // slice.GetComponent<MeshRenderer>()
                //      ?.material.SetVector("CutPlaneOrigin", Vector3.positiveInfinity);

                cuttable.Cut(GetNormal(), transform.position);
            }
        }

        Vector3 GetNormal()
        {
            return useLocalNormal
                ? transform.TransformDirection(normal).normalized
                : normal.normalized;
        }

        void OnDrawGizmosSelected()
        {
            Vector3 n = GetNormal();

            Quaternion rot = Quaternion.FromToRotation(Vector3.up, n);

            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(
                transform.position,
                rot,
                Vector3.one
            );

            Gizmos.DrawWireCube(
                Vector3.zero,
                new Vector3(size * 2f, epsilonHeight * 2f, size * 2f)
            );

            // Draw normal for clarity
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                transform.position,
                transform.position + n * size
            );
        }
    }
}
