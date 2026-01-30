using System.Collections.Generic;
using UnityEngine;

public class RepelOnTouch : MonoBehaviour
{
    [Header("Repel Settings")]
    public float repelForce = 15f;
    public bool pushOnlyOnce = false;

    // Tracks which rigidbodies have already been pushed
    private HashSet<Rigidbody> pushedBodies = new HashSet<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb == null)
            return;

        if (pushOnlyOnce && pushedBodies.Contains(rb))
            return;

        // Direction away from this object
        Vector3 direction = (rb.position - transform.position).normalized;

        rb.AddForce(direction * repelForce, ForceMode.Impulse);

        if (pushOnlyOnce)
            pushedBodies.Add(rb);
    }

    // Optional: support trigger volumes too
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null)
            return;

        if (pushOnlyOnce && pushedBodies.Contains(rb))
            return;

        Vector3 direction = (rb.position - transform.position).normalized;
        rb.AddForce(direction * repelForce, ForceMode.Impulse);

        if (pushOnlyOnce)
            pushedBodies.Add(rb);
    }
}
