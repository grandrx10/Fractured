using System;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Jump Settings")]
    public float launchForce = 15f;

    public bool continuous;
    
    [Header("Cooldown")]
    public float cooldown = 1.0f;
    
    [Header("Detection")]
    public LayerMask playerLayer;
    [Header("Audio")]
    public AudioClip launchSound;
    [Range(0f, 5f)]
    public float soundVolume = 1f;

    
    // Track cooldown per rigidbody to prevent issues with multiple players
    private System.Collections.Generic.Dictionary<Rigidbody, float> lastUseTimes = 
        new System.Collections.Generic.Dictionary<Rigidbody, float>();

    private void OnCollisionEnter(Collision collision)
    {
        TryLaunch(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryLaunch(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (continuous) TryLaunch(other.gameObject);
    }

    private void TryLaunch(GameObject obj)
    {
        // Check layer mask first
        if (((1 << obj.layer) & playerLayer) == 0)
        {
            if (continuous) Debug.Log("continuous blow up cpu");
            return;
        
        // Search for Rigidbody in parent hierarchy
        }
        
        // Search in parent hierarchy for Rigidbody
        Rigidbody rb = obj.GetComponentInParent<Rigidbody>();
        if (rb == null)
            return;
        
        // Check cooldown per rigidbody
        if (lastUseTimes.ContainsKey(rb))
        {
            if (Time.time < lastUseTimes[rb] + cooldown)
                return;
        }
        
        // Update last use time for this rigidbody
        lastUseTimes[rb] = Time.time;
        
        // Get the launch direction (jump pad's up direction)
        Vector3 launchDirection = transform.up;
        
        // Cancel existing vertical velocity to ensure consistent jump height
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 verticalVelocity = Vector3.Project(currentVelocity, launchDirection);
        
        // Only cancel upward velocity if moving against the launch direction
        // This prevents canceling momentum when already moving in the right direction
        if (Vector3.Dot(verticalVelocity, launchDirection) < 0)
        {
            rb.linearVelocity -= verticalVelocity;
        }
        else
        {
            // If already moving in launch direction, remove it for consistency
            rb.linearVelocity -= verticalVelocity;
        }
        
        // Apply launch impulse
        rb.AddForce(launchDirection * launchForce, ForceMode.VelocityChange);
        if (launchSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(launchSound, transform.position, soundVolume);
        }
    }
    
    // Clean up old entries to prevent memory leaks
    private void LateUpdate()
    {
        // Remove entries older than cooldown + buffer
        float cleanupThreshold = Time.time - (cooldown + 5f);
        var keysToRemove = new System.Collections.Generic.List<Rigidbody>();
        
        foreach (var kvp in lastUseTimes)
        {
            if (kvp.Value < cleanupThreshold || kvp.Key == null)
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        
        foreach (var key in keysToRemove)
        {
            lastUseTimes.Remove(key);
        }
    }
}