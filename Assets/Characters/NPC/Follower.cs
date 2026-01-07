using UnityEngine;

namespace Characters
{
    public class Follower : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Followable targetToFollow;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private int nodesToStayBehind = 3;

        [Header("Ground Detection")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private float followerHeight = 1f;

        [Header("Midair Settings")]
        [SerializeField] private float nodeReachThreshold = 0.1f;
        [SerializeField] private float airSpeedMultiplier = 1f;

        private Vector3 moveVelocity;
        private float groundedTimer;
        private const float groundedGraceTime = 0.1f;

        private int currentTargetNodeIndex;
        private int lockedMidairTargetIndex = -1;

        void OnEnable()
        {
            if (targetToFollow != null)
                targetToFollow.OnNodesRemoved += HandleNodesRemoved;
        }

        void OnDisable()
        {
            if (targetToFollow != null)
                targetToFollow.OnNodesRemoved -= HandleNodesRemoved;
        }

        public void SetFollowable(Followable newTarget)
        {
            if (targetToFollow != null)
                targetToFollow.OnNodesRemoved -= HandleNodesRemoved;

            targetToFollow = newTarget;
            currentTargetNodeIndex = 0;
            lockedMidairTargetIndex = -1;

            if (targetToFollow != null)
                targetToFollow.OnNodesRemoved += HandleNodesRemoved;

            DisableAllColliders();
        }

        void Update()
        {
            if (targetToFollow == null) return;
            FollowPath();
        }

        private void HandleNodesRemoved(int removedCount)
        {
            currentTargetNodeIndex = Mathf.Max(0, currentTargetNodeIndex - removedCount);
            lockedMidairTargetIndex = lockedMidairTargetIndex < 0
                ? -1
                : Mathf.Max(0, lockedMidairTargetIndex - removedCount);
        }

        private void FollowPath()
        {
            var pathNodes = targetToFollow.PathNodes;
            if (pathNodes.Count == 0) return;

            bool groundedNow = CheckIfGrounded();
            groundedTimer = groundedNow ? groundedGraceTime : groundedTimer - Time.deltaTime;
            bool isGrounded = groundedTimer > 0f;

            int lastIndex = pathNodes.Count - 1;
            int desiredNodeIndex = currentTargetNodeIndex;

            if (isGrounded)
            {
                int maxAllowedIndex = Mathf.Max(0, lastIndex - nodesToStayBehind);

                if (currentTargetNodeIndex < maxAllowedIndex)
                    desiredNodeIndex = maxAllowedIndex;

                lockedMidairTargetIndex = -1;
            }
            else
            {
                if (lockedMidairTargetIndex < 0)
                    lockedMidairTargetIndex = lastIndex;

                desiredNodeIndex = lockedMidairTargetIndex;
            }

            currentTargetNodeIndex = Mathf.Clamp(
                Mathf.Max(currentTargetNodeIndex, desiredNodeIndex),
                0,
                lastIndex
            );

            Vector3 targetPosition = pathNodes[currentTargetNodeIndex];

            // =========================
            // MOVEMENT (KEY FIX)
            // =========================
            if (isGrounded)
            {
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    targetPosition,
                    ref moveVelocity,
                    0.15f,
                    moveSpeed
                );
            }
            else
            {
                Vector3 delta = targetPosition - transform.position;
                float distance = delta.magnitude;

                if (distance > 0.001f)
                {
                    Vector3 step = delta.normalized * (moveSpeed * airSpeedMultiplier * Time.deltaTime);
                    if (step.magnitude > distance)
                        step = delta;

                    transform.position += step;
                    moveVelocity = step / Time.deltaTime;
                }
            }

            // Rotation
            Vector3 flatVelocity = new(moveVelocity.x, 0f, moveVelocity.z);
            if (flatVelocity.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatVelocity);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            // Advance midair index only when reaching node
            if (!isGrounded &&
                Vector3.Distance(transform.position, targetPosition) < nodeReachThreshold)
            {
                lockedMidairTargetIndex = Mathf.Min(
                    lockedMidairTargetIndex + 1,
                    lastIndex
                );
            }
        }

        private bool CheckIfGrounded()
        {
            Vector3 rayOrigin = transform.position + Vector3.up * (followerHeight * 0.5f);
            return Physics.Raycast(
                rayOrigin,
                Vector3.down,
                followerHeight * 0.5f + groundCheckDistance,
                groundLayer
            );
        }

        private void DisableAllColliders()
        {
            foreach (var col in GetComponentsInChildren<Collider>())
                col.enabled = false;
        }

        private void OnDrawGizmos()
        {
            if (targetToFollow == null) return;
            var pathNodes = targetToFollow.PathNodes;
            if (pathNodes.Count == 0) return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pathNodes[currentTargetNodeIndex], 0.25f);
            Gizmos.DrawLine(transform.position, pathNodes[currentTargetNodeIndex]);
        }
    }
}
