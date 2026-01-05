using System.Collections.Generic;
using UnityEngine;

public class PulleyPlatform : MonoBehaviour
{
    [Header("Pulley Link")]
    public PulleyPlatform otherPlatform;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Height Limits (relative to start)")]
    public float minOffset = -5f;
    public float maxOffset = 5f;

    private readonly HashSet<Rigidbody> bodiesOnPlatform = new HashSet<Rigidbody>();
    private float baseY;

    public float TotalMass
    {
        get
        {
            float mass = 0f;
            foreach (var rb in bodiesOnPlatform)
            {
                if (rb != null)
                    mass += rb.mass;
            }
            return mass;
        }
    }

    private void Awake()
    {
        baseY = transform.position.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
            bodiesOnPlatform.Add(other.attachedRigidbody);
        Debug.Log("ENTERED!");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
            bodiesOnPlatform.Remove(other.attachedRigidbody);
    }

    private void FixedUpdate()
{
    if (otherPlatform == null)
        return;

    float myMass = TotalMass;
    float otherMass = otherPlatform.TotalMass;

    float delta;

    // CASE 1: Equal mass → return to equilibrium
    if (Mathf.Approximately(myMass, otherMass))
    {
        float myOffset = transform.position.y - baseY;

        // Already balanced
        if (Mathf.Abs(myOffset) < 0.01f)
            return;

        // Move back toward base
        float direction = myOffset > 0f ? -1f : 1f;
        delta = direction * moveSpeed * Time.fixedDeltaTime;
    }
    // CASE 2: Unequal mass → heavier side goes down
    else
    {
        float direction = myMass > otherMass ? -1f : 1f;
        delta = direction * moveSpeed * Time.fixedDeltaTime;
    }

    float newMyY = transform.position.y + delta;
    float newOtherY = otherPlatform.transform.position.y - delta;

    // Limits
    float myMinY = baseY + minOffset;
    float myMaxY = baseY + maxOffset;
    float otherMinY = otherPlatform.baseY + otherPlatform.minOffset;
    float otherMaxY = otherPlatform.baseY + otherPlatform.maxOffset;

    bool myBlocked =
        (newMyY < myMinY && delta < 0f) ||
        (newMyY > myMaxY && delta > 0f);

    bool otherBlocked =
        (newOtherY < otherMinY && -delta < 0f) ||
        (newOtherY > otherMaxY && -delta > 0f);

    if (myBlocked || otherBlocked)
        return;

    transform.position = new Vector3(
        transform.position.x,
        newMyY,
        transform.position.z
    );

    otherPlatform.transform.position = new Vector3(
        otherPlatform.transform.position.x,
        newOtherY,
        otherPlatform.transform.position.z
    );
}

}
