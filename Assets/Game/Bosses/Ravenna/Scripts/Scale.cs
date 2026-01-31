using System.Collections.Generic;
using Cards.Environments;
using UnityEngine;
using Utils;

public class ScalePlatform : MonoBehaviour
{
    public bool allowMove = true;

    [Header("Pulley Link")]
    public ScalePlatform otherPlatform;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Height Limits (relative to start)")]
    public float minOffset = -5f;
    public float maxOffset = 5f;
    public LayerMask validLayers;
    [Header("Rope Visual")]
    public Transform ropeTransform;
    public float baseYScale = 1f;
    public float yScalePerUnit = 0.2f;

    private readonly HashSet<Rigidbody> bodiesOnPlatform = new();
    private float baseY;
    private Rigidbody rb;
    public float HeightDelta => baseY - transform.position.y;
    public float TotalMass
    {
        get
        {
            float mass = 0f;
            foreach (var body in bodiesOnPlatform)
                if (body) mass += body.mass;
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
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        baseY = transform.position.y;
        if (ropeTransform)
            baseYScale = ropeTransform.localScale.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (other.attachedRigidbody && PhysicsHelper.IsInMask(other.attachedRigidbody.gameObject.layer, validLayers))
            bodiesOnPlatform.Add(other.attachedRigidbody);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
        if (other.attachedRigidbody)
            bodiesOnPlatform.Remove(other.attachedRigidbody);
    }

    float ComputeRopeVelocity()
    {
        if (!allowMove)
            return -(transform.position.y - baseY) * moveSpeed;

        float myMass = TotalMass;
        float otherMass = otherPlatform.TotalMass;

        if (Mathf.Approximately(myMass, otherMass))
            return -(transform.position.y - baseY) * moveSpeed;

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

        if (y <= minY && ropeVelocity < 0f)
            ropeVelocity = 0f;
        else if (y >= maxY && ropeVelocity > 0f)
            ropeVelocity = 0f;

        Vector3 nextPos = rb.position + Vector3.up * (ropeVelocity * Time.fixedDeltaTime);
        rb.MovePosition(nextPos);

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
