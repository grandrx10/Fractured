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

    private float lastUseTime = -Mathf.Infinity;

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
        if (Time.time < lastUseTime + cooldown)
            return;

        if (((1 << obj.layer) & playerLayer) == 0)
        {
            if (continuous) Debug.Log("continuous blow up cpu");
            return;
        }
        
        // Search in parent hierarchy for Rigidbody
        Rigidbody rb = obj.GetComponentInParent<Rigidbody>();
        if (rb == null)
        {
            Debug.Log("No Rigidbody found in parent hierarchy!");
            return;
        }
        
        Vector3 up = transform.up;

        // Remove current velocity along the launch direction
        rb.linearVelocity -= Vector3.Project(rb.linearVelocity, up);

        // Apply launch force in local up direction
        rb.linearVelocity += up * launchForce;
    }
}