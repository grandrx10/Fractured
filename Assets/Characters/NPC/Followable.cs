using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class Followable : MonoBehaviour
    {
        [Header("Path Recording Settings")]
        [SerializeField] private float minDistanceBetweenNodes = 1.5f;
        [SerializeField] private int maxNodes = 100;

        private readonly List<Vector3> pathNodes = new();

        public IReadOnlyList<Vector3> PathNodes => pathNodes;

        public event Action<int> OnNodesRemoved; // how many nodes were deleted

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
            Vector3 lastNode = pathNodes[^1];

            if (Vector3.Distance(transform.position, lastNode) < minDistanceBetweenNodes)
                return;

            pathNodes.Add(transform.position);

            int removed = 0;
            while (pathNodes.Count > maxNodes)
            {
                pathNodes.RemoveAt(0);
                removed++;
            }

            if (removed > 0)
                OnNodesRemoved?.Invoke(removed);
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
