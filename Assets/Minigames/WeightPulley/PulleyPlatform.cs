using System.Collections.Generic;
using Cards.Environments;
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

    [Header("Rope Visual")]
    public Transform ropeTransform;
    public float baseYScale = 1f;
    public float yScalePerUnit = 0.2f;

    private readonly HashSet<Rigidbody> bodiesOnPlatform = new HashSet<Rigidbody>();
    private float baseY;
    private Rigidbody rb;

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
        GlobalWorldManager.OnPreLoadNewScene += Init;
    }

    private void Init(CardEnv env)
    {
        GlobalWorldManager.OnPreLoadNewScene -= Init;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        baseY = transform.position.y;
        baseYScale = ropeTransform.localScale.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
            bodiesOnPlatform.Add(other.attachedRigidbody);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
            bodiesOnPlatform.Remove(other.attachedRigidbody);
    }

    float ComputeRopeVelocity()
    {
        float myMass = TotalMass;
        float otherMass = otherPlatform.TotalMass;

        if (Mathf.Approximately(myMass, otherMass))
        {
            float offset = transform.position.y - baseY;
            return -offset * moveSpeed;
        }

        return (otherMass - myMass) * moveSpeed;
    }

    void FixedUpdate()
    {
        if (!rb || !otherPlatform)
            return;

        float ropeVelocity = ComputeRopeVelocity();
        float y = rb.position.y;

        float minY = baseY + minOffset;
        float maxY = baseY + maxOffset;

        // Only stop velocity if it would move further outside
        if (y <= minY && ropeVelocity < 0f)
            ropeVelocity = 0f;
        else if (y >= maxY && ropeVelocity > 0f)
            ropeVelocity = 0f;

        rb.linearVelocity = new Vector3(0f, ropeVelocity, 0f);

        UpdateRopeVisual();
    }

    
    void OnDrawGizmos()
    {
        float y = Application.isPlaying ? baseY : transform.position.y;

        float minY = y + minOffset;
        float maxY = y + maxOffset;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            new Vector3(transform.position.x - 0.5f, minY, transform.position.z),
            new Vector3(transform.position.x + 0.5f, minY, transform.position.z)
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            new Vector3(transform.position.x - 0.5f, maxY, transform.position.z),
            new Vector3(transform.position.x + 0.5f, maxY, transform.position.z)
        );
    }


    void UpdateRopeVisual()
    {
        if (!ropeTransform)
            return;

        float displacement = baseY - transform.position.y;
        float newYScale = baseYScale + displacement * yScalePerUnit;

        Vector3 scale = ropeTransform.localScale;
        scale.y = newYScale;
        ropeTransform.localScale = scale;
    }
}
