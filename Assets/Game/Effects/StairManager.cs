using UnityEngine;

namespace Game.Effects
{
    public class StairManager : MonoBehaviour
    {
        public GameObject stairPrefab;

        [Header("Stair Field")]
        public int stepsForward = 30;
        public float stepHeight = 0.25f;
        public float stepDepth = 0.5f;

        [ContextMenu("Spawn Stairs")]
        void Spawn()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            SpawnStairField();
        }

        void SpawnStairField()
        {
            Vector3 forward =
                Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

            Vector3 right = Vector3.Cross(Vector3.up, forward);

            for (int z = 0; z < stepsForward; z++)
            {
                
                Vector3 pos =
                    transform.position +
                    forward * (z * stepDepth) +
                    Vector3.up * (z * stepHeight);

                GameObject stair = Instantiate(
                    stairPrefab,
                    pos,
                    Quaternion.LookRotation(forward),
                    transform
                );
                
            }
        }
    }
}