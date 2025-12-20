using System.Collections;
using UnityEngine;
using Game.Bosses.Projectiles;

public class VerticalLaserWall : MonoBehaviour
{
    [Header("References")]
    public GameObject laserCylinder;      // actual laser object (Cylinder)
    public GameObject warningPrefab;      // ground warning prefab

    [Header("Laser Settings")]
    public float finalRadius = 5f;        // target width (X/Z scale)
    public float chargeDuration = 3f;     // time before laser expands
    public float activeDuration = 3f;     // time laser stays active after expanding

    [Header("Warning")]
    public float warningRadius = 5f;
    public float warningDuration = 3f;

    private void Start()
    {
        StartCoroutine(LaserRoutine());
    }

    private IEnumerator LaserRoutine()
    {
        // 1) Spawn warning directly under the laser
        if (warningPrefab != null)
        {
            Vector3 warningPos = GetGroundPosition(transform.position);

            Warning w = Instantiate(warningPrefab, warningPos, Quaternion.identity)
                .GetComponent<Warning>();

            w.Initialize(
                warningRadius,
                0.5f,
                Warning.WarningType.Grounded,
                warningDuration
            );
        }

        // 2) Wait for charge-up
        yield return new WaitForSeconds(chargeDuration);
        laserCylinder.SetActive(true);

        // 3) Expand laser radius
        if (laserCylinder != null)
        {
            Vector3 scale = laserCylinder.transform.localScale;
            scale.x = finalRadius;
            scale.z = finalRadius;
            laserCylinder.transform.localScale = scale;
        }

        // 4) Laser active time
        yield return new WaitForSeconds(activeDuration);

        // 5) Destroy laser
        if (laserCylinder != null)
            Destroy(laserCylinder);

        Destroy(gameObject);
    }

    private Vector3 GetGroundPosition(Vector3 origin)
    {
        if (Physics.Raycast(origin + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f, LayerMask.GetMask("Ground")))
        {
            return hit.point;
        }

        return origin;
    }
}
