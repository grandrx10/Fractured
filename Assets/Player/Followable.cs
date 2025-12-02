using UnityEngine;
using System.Collections.Generic;

public class Followable : MonoBehaviour
{
    [Header("Path Recording Settings")]
    [SerializeField] private float minDistanceBetweenNodes = 1.5f; // Minimum distance to record a new node
    [SerializeField] private int maxNodes = 100; // Maximum number of nodes to keep
    
    private List<Vector3> pathNodes = new List<Vector3>();
    private int totalNodesRemoved = 0; // Track how many nodes have been removed from the start
    
    public List<Vector3> PathNodes => pathNodes; // Public accessor for followers
    public int TotalNodesRemoved => totalNodesRemoved; // Public accessor for offset
    
    void Start()
    {
        // Record the starting position
        pathNodes.Add(transform.position);
    }
    
    void Update()
    {
        RecordPosition();
    }
    
    private void RecordPosition()
    {
        // Check if we should record a new node
        if (pathNodes.Count == 0)
        {
            pathNodes.Add(transform.position);
            return;
        }
        
        Vector3 lastNode = pathNodes[pathNodes.Count - 1];
        float distanceFromLastNode = Vector3.Distance(transform.position, lastNode);
        
        // Only record if player has moved far enough from the last node
        if (distanceFromLastNode >= minDistanceBetweenNodes)
        {
            pathNodes.Add(transform.position);
            
            // Remove oldest nodes if we exceed the limit
            if (pathNodes.Count > maxNodes)
            {
                pathNodes.RemoveAt(0);
                totalNodesRemoved++;
            }
        }
    }
    
    // Optional: Visualize the path in the editor
    private void OnDrawGizmos()
    {
        if (pathNodes == null || pathNodes.Count < 2) return;
        
        Gizmos.color = Color.yellow;
        for (int i = 0; i < pathNodes.Count - 1; i++)
        {
            Gizmos.DrawLine(pathNodes[i], pathNodes[i + 1]);
            Gizmos.DrawSphere(pathNodes[i], 0.2f);
        }
        
        // Draw the last node
        Gizmos.DrawSphere(pathNodes[pathNodes.Count - 1], 0.2f);
    }
}