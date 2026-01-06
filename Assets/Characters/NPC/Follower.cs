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

        private Vector3 moveVelocity;
        private float groundedTimer;
        private const float groundedGraceTime = 0.1f;

        void Update()
        {
            if (targetToFollow == null) return;

            FollowPath();
        }

        public void SetFollowable(Followable newTarget)
        {
            targetToFollow = newTarget;
            DisableAllColliders();
        }

        private void FollowPath()
        {
            var pathNodes = targetToFollow.PathNodes;
            if (pathNodes.Count == 0) return;

            bool groundedNow = CheckIfGrounded();
            if (groundedNow)
                groundedTimer = groundedGraceTime;
            else
                groundedTimer -= Time.deltaTime;

            bool isGrounded = groundedTimer > 0f;

            int targetNodeIndex;

            if (!isGrounded)
            {
                // Midair: catch up
                targetNodeIndex = pathNodes.Count - 1;
            }
            else
            {
                if (pathNodes.Count < nodesToStayBehind + 1) return;
                targetNodeIndex = pathNodes.Count - nodesToStayBehind - 1;
            }

            targetNodeIndex = Mathf.Clamp(targetNodeIndex, 0, pathNodes.Count - 1);

            Vector3 targetPosition = pathNodes[targetNodeIndex];

            float smoothTime = isGrounded ? 0.15f : 0.08f;
            float maxSpeed = isGrounded ? moveSpeed : moveSpeed * 1.2f;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref moveVelocity,
                smoothTime,
                maxSpeed
            );

            // Rotate using velocity (prevents jitter near nodes)
            Vector3 flatVelocity = new Vector3(moveVelocity.x, 0f, moveVelocity.z);
            if (flatVelocity.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatVelocity);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
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
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
                col.enabled = false;
        }

        private void OnDrawGizmos()
        {
            if (targetToFollow == null) return;

            var pathNodes = targetToFollow.PathNodes;
            if (pathNodes.Count == 0) return;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, pathNodes[pathNodes.Count - 1]);
            Gizmos.DrawWireSphere(pathNodes[pathNodes.Count - 1], 0.25f);
        }
    }
}
