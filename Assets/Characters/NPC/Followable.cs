using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class Followable : MonoBehaviour
    {
        [Header("Path Recording Settings")]
        [SerializeField] private float minDistanceBetweenNodes = 1.5f;
        [SerializeField] private int maxNodes = 100;

        private readonly List<Vector3> pathNodes = new List<Vector3>();

        public List<Vector3> PathNodes => pathNodes;

        void Start()
        {
            pathNodes.Add(transform.position);
        }

        void Update()
        {
            RecordPosition();
        }

        private void RecordPosition()
        {
            if (pathNodes.Count == 0)
            {
                pathNodes.Add(transform.position);
                return;
            }

            Vector3 lastNode = pathNodes[pathNodes.Count - 1];
            if (Vector3.Distance(transform.position, lastNode) >= minDistanceBetweenNodes)
            {
                pathNodes.Add(transform.position);

                if (pathNodes.Count > maxNodes)
                {
                    pathNodes.RemoveAt(0);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (pathNodes.Count < 2) return;

            Gizmos.color = Color.yellow;
            for (int i = 0; i < pathNodes.Count - 1; i++)
            {
                Gizmos.DrawLine(pathNodes[i], pathNodes[i + 1]);
                Gizmos.DrawSphere(pathNodes[i], 0.15f);
            }
        }
    }
}
