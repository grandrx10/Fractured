// LightPlate.cs

using System;
using UnityEngine;
using System.Collections.Generic;

public class LightPlate : MonoBehaviour
{
    [Header("Neighbor Detection")]
    [SerializeField] private float neighborDetectionSize = 1.5f;
    [SerializeField] private bool detectNeighborsInX = true;
    [SerializeField] private bool detectNeighborsInY = false;
    [SerializeField] private bool detectNeighborsInZ = true;
    
    [Header("Animation")]
    [SerializeField] private float pressDepth = 0.1f;
    [SerializeField] private float animationSpeed = 10f;
    [SerializeField] private Transform plateVisual;
    
    [Header("Visual")]
    [SerializeField] private Renderer plateRenderer;
    [SerializeField] private Material unpressedMaterial;
    [SerializeField] private Material pressedMaterial;
    
    [Header("Gizmos")]
    [SerializeField] private bool showNeighborGizmos = false;
    
    private List<LightPlate> neighbors = new List<LightPlate>();
    private bool isPressed = false;
    
    public bool IsPressed => isPressed;
    
    private void Awake()
    {
        // Use the main transform if no visual is specified
        if (plateVisual == null)
        {
            plateVisual = transform;
        }
        
        // Get renderer if not assigned
        if (plateRenderer == null)
        {
            plateRenderer = GetComponentInChildren<Renderer>();
        }
        
        UpdateVisual();
        
        // Gather neighbors after a frame to ensure positions are set
        Invoke(nameof(GatherNeighbors), 0.1f);
    }
    
    private void GatherNeighbors()
    {
        neighbors.Clear();
        
        // Find all other light plates
        LightPlate[] allPlates = FindObjectsOfType<LightPlate>();
        
        foreach (var plate in allPlates)
        {
            if (plate == this) continue;
            
            // Get direction in local space
            Vector3 worldDirectionToPlate = plate.transform.position - transform.position;
            Vector3 localDirectionToPlate = transform.InverseTransformDirection(worldDirectionToPlate);
            float distance = localDirectionToPlate.magnitude;
            
            // Check if plate is within detection range
            if (distance > neighborDetectionSize * 2f) continue;
            
            // Normalize direction
            localDirectionToPlate.Normalize();
            
            // Check if neighbor is in allowed axes (in local space)
            bool isNeighbor = false;
            
            if (detectNeighborsInX && Mathf.Abs(localDirectionToPlate.x) > 0.7f)
            {
                isNeighbor = true;
            }
            
            if (detectNeighborsInY && Mathf.Abs(localDirectionToPlate.y) > 0.7f)
            {
                isNeighbor = true;
            }
            
            if (detectNeighborsInZ && Mathf.Abs(localDirectionToPlate.z) > 0.7f)
            {
                isNeighbor = true;
            }
            
            if (isNeighbor)
            {
                neighbors.Add(plate);
            }
        }
        
        Debug.Log($"{gameObject.name} found {neighbors.Count} neighbors");
    }
    
    private void Update()
    {
        // Calculate target position based on current pressed state
        Vector3 targetLocalPosition = GetTargetLocalPosition();
        _cd -= Time.deltaTime;
        // Smooth animation
        if (plateVisual != null)
        {
            plateVisual.localPosition = Vector3.Lerp(
                plateVisual.localPosition,
                targetLocalPosition,
                Time.deltaTime * animationSpeed
            );
        }
    }
    
    private Vector3 GetTargetLocalPosition()
    {
        // Calculate the original position by adding back the press depth if currently pressed
        Vector3 currentLocal = plateVisual.localPosition;
        
        if (isPressed)
        {
            // We want to go down by pressDepth from the unpressed position
            return new Vector3(currentLocal.x, -pressDepth, currentLocal.z);
        }
        else
        {
            // We want to return to original position (0 offset)
            return new Vector3(currentLocal.x, 0, currentLocal.z);
        }
    }

    private float _cd;
    
    private void OnCollisionEnter(Collision other)
    {
        if (_cd < 0)
        {
            TogglePlate(false);
            _cd = 0.5f;
        }
    }

    public void TogglePlate(bool thisOnly)
    {
        // Toggle this plate
        SetPressed(!isPressed);
        if (thisOnly) return;
        // Toggle all neighbors (lights-out style)
        foreach (var neighbor in neighbors)
        {
            neighbor.SetPressed(!neighbor.isPressed);
        }
    }
    
    public void SetPressed(bool pressed)
    {
        isPressed = pressed;
        
        // Update material
        UpdateVisual();
    }
    
    private void UpdateVisual()
    {
        if (plateRenderer != null)
        {
            plateRenderer.material = isPressed ? pressedMaterial : unpressedMaterial;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showNeighborGizmos) return;
        
        // Draw detection range in local space
        Gizmos.color = Color.yellow;
        
        if (detectNeighborsInX)
        {
            Vector3 rightLocal = transform.TransformDirection(Vector3.right);
            Vector3 leftLocal = transform.TransformDirection(Vector3.left);
            
            Gizmos.DrawWireCube(transform.position + rightLocal * neighborDetectionSize, 
                Vector3.one * 0.3f);
            Gizmos.DrawWireCube(transform.position + leftLocal * neighborDetectionSize, 
                Vector3.one * 0.3f);
        }
        
        if (detectNeighborsInY)
        {
            Vector3 upLocal = transform.TransformDirection(Vector3.up);
            Vector3 downLocal = transform.TransformDirection(Vector3.down);
            
            Gizmos.DrawWireCube(transform.position + upLocal * neighborDetectionSize, 
                Vector3.one * 0.3f);
            Gizmos.DrawWireCube(transform.position + downLocal * neighborDetectionSize, 
                Vector3.one * 0.3f);
        }
        
        if (detectNeighborsInZ)
        {
            Vector3 forwardLocal = transform.TransformDirection(Vector3.forward);
            Vector3 backLocal = transform.TransformDirection(Vector3.back);
            
            Gizmos.DrawWireCube(transform.position + forwardLocal * neighborDetectionSize, 
                Vector3.one * 0.3f);
            Gizmos.DrawWireCube(transform.position + backLocal * neighborDetectionSize, 
                Vector3.one * 0.3f);
        }
        
        // Draw lines to neighbors (only in play mode when neighbors are gathered)
        if (Application.isPlaying && neighbors != null)
        {
            Gizmos.color = isPressed ? Color.green : Color.red;
            foreach (var neighbor in neighbors)
            {
                if (neighbor != null)
                {
                    Gizmos.DrawLine(transform.position, neighbor.transform.position);
                }
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showNeighborGizmos) return;
        
        // Draw a sphere showing the maximum detection range
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, neighborDetectionSize * 2f);
    }
}