using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    [Header("Laser Settings")]
    public float maxDistancePerReflection = 20f;
    public int maxReflections = 10;
    public LayerMask laserHitMask;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        DrawLaser();
    }

    void DrawLaser()
    {
        List<Vector3> points = new List<Vector3>();

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        points.Add(origin);

        for (int i = 0; i < maxReflections; i++)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistancePerReflection, laserHitMask))
            {
                points.Add(hit.point);

                // Check if hit a mirror
                // Check for laser receiver
                LaserReceiver receiver = hit.collider.GetComponentInParent<LaserReceiver>();
                if (receiver != null)
                {
                    receiver.RegisterLaserHit();
                    break;
                }

                // Check for mirror
                Mirror mirror = hit.collider.GetComponentInParent<Mirror>();
                if (mirror == null)
                    break;
                // Reflect direction
                direction = Vector3.Reflect(direction, hit.normal).normalized;

                // Offset slightly to avoid self-hit
                origin = hit.point + direction * 0.01f;
            }
            else
            {
                // No hit, extend laser forward
                points.Add(origin + direction * maxDistancePerReflection);
                break;
            }
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
