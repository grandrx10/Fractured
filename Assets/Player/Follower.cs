using UnityEngine;

public class Follower : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Followable targetToFollow;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float nodeReachedThreshold = 0.5f; // Distance to consider node reached
    [SerializeField] private int nodesToStayBehind = 3; // Stay 3 nodes behind the player
    
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float followerHeight = 1f; // Height of the follower for ground check offset
    [SerializeField] private bool showDebugLogs = false;
    
    private int currentNodeIndex = 0;
    private int lastKnownNodesRemoved = 0;
    private bool isTransitioning = false;
    private Vector3 transitionStartPosition;
    
    void Update()
    {
        if (targetToFollow == null)
        {
            isTransitioning = false;
            return;
        }
        
        if (isTransitioning)
        {
            TransitionToPath();
        }
        else
        {
            FollowPath();
        }
    }
    
    /// <summary>
    /// Sets a new followable target and initiates smooth transition to its path
    /// </summary>
    public void SetFollowable(Followable newTarget)
    {
        if (newTarget == null)
        {
            targetToFollow = null;
            isTransitioning = false;
            return;
        }

        targetToFollow = newTarget;

        // Disable all colliders immediately
        DisableAllColliders();

        // Start transition if we have a valid path
        if (targetToFollow.PathNodes.Count > 0)
        {
            isTransitioning = true;
            lastKnownNodesRemoved = targetToFollow.TotalNodesRemoved;

            // Determine target node based on grounded state
            bool isGrounded = CheckIfGrounded();
            if (!isGrounded)
            {
                currentNodeIndex = Mathf.Max(0, targetToFollow.PathNodes.Count - 1);
            }
            else
            {
                currentNodeIndex = Mathf.Clamp(targetToFollow.PathNodes.Count - nodesToStayBehind - 1, 0, targetToFollow.PathNodes.Count - 1);
            }
        }
    }

    private void TransitionToPath()
    {
        var pathNodes = targetToFollow.PathNodes;
        if (pathNodes.Count == 0) return;

        Vector3 targetPosition = pathNodes[currentNodeIndex];

        // Move towards the target position at moveSpeed
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // Clamp the movement so we don't overshoot
        float moveStep = moveSpeed * Time.deltaTime;
        if (moveStep >= distanceToTarget)
        {
            transform.position = targetPosition;
            isTransitioning = false;
        }
        else
        {
            transform.position += direction * moveStep;
        }

        // Rotate towards target
        if (direction != Vector3.zero)
        {
            Vector3 flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
            if (flatDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }


    
    private void FollowPath()
    {
        var pathNodes = targetToFollow.PathNodes;
        
        // Adjust currentNodeIndex if nodes were removed from the beginning
        int nodesRemovedSinceLastFrame = targetToFollow.TotalNodesRemoved - lastKnownNodesRemoved;
        if (nodesRemovedSinceLastFrame > 0)
        {
            currentNodeIndex -= nodesRemovedSinceLastFrame;
            if (currentNodeIndex < 0) currentNodeIndex = 0;
            lastKnownNodesRemoved = targetToFollow.TotalNodesRemoved;
        }
        
        bool isGrounded = CheckIfGrounded();
        float travelSpeed = moveSpeed;
        // When midair, ignore the nodesToStayBehind policy and catch up
        int targetNodeIndex;
        if (!isGrounded)
        {
            targetNodeIndex = pathNodes.Count - 1; // Go to the latest node when airborne
            travelSpeed = (float) (moveSpeed * 1.2);
        }
        else
        {   
            // Make sure we have enough nodes and stay behind by the specified amount when grounded
            if (pathNodes.Count < nodesToStayBehind + 1) return;
            targetNodeIndex = pathNodes.Count - nodesToStayBehind - 1;
        }
        
        // NEVER go backwards - only advance or stay at current node
        if (currentNodeIndex > targetNodeIndex)
        {
            targetNodeIndex = currentNodeIndex;
        }
        
        if (currentNodeIndex < 0 || currentNodeIndex >= pathNodes.Count) return;
        
        Vector3 targetPosition = pathNodes[currentNodeIndex];
        
        if (showDebugLogs)
        {
            Debug.Log($"CurrentNode: {currentNodeIndex} | TargetNode: {targetNodeIndex} | TotalNodes: {pathNodes.Count} | Grounded: {isGrounded} | NodesRemoved: {targetToFollow.TotalNodesRemoved}");
        }
        
        // Move towards the current target node
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distanceToNode = Vector3.Distance(transform.position, targetPosition);
        
        if (distanceToNode > nodeReachedThreshold)
        {
            // Move towards the node
            transform.position += direction * travelSpeed * Time.deltaTime;
            
            // Rotate only on Y axis (XZ plane) - no tilting
            if (direction != Vector3.zero)
            {
                Vector3 flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
                if (flatDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            // Node reached, move to next node if available
            if (currentNodeIndex < targetNodeIndex)
            {
                currentNodeIndex++;
            } 
        }
    }
    
    private bool CheckIfGrounded()
    {
        // Raycast downward from the center of the follower
        Vector3 rayOrigin = transform.position + Vector3.up * (followerHeight * 0.5f);
        return Physics.Raycast(rayOrigin, Vector3.down, followerHeight * 0.5f + groundCheckDistance, groundLayer);
    }
    
    // Optional: Visualize the follower's target in the editor
    private void OnDrawGizmos()
    {
        if (targetToFollow == null) return;
        
        var pathNodes = targetToFollow.PathNodes;
        if (pathNodes.Count < nodesToStayBehind + 1) return;
        
        int targetNodeIndex = pathNodes.Count - nodesToStayBehind - 1;
        if (currentNodeIndex >= 0 && currentNodeIndex < pathNodes.Count)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, pathNodes[currentNodeIndex]);
            Gizmos.DrawWireSphere(pathNodes[currentNodeIndex], 0.3f);
        }
    }

    private void DisableAllColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
    }
}