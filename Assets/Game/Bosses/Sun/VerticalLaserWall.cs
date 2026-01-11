using System.Collections;
using UnityEngine;
using Game.Bosses.Projectiles;

public class VerticalLaserWall : MonoBehaviour
{
    [Header("References")]
    public GameObject laserCylinder;      // actual laser object (Cylinder)
    public GameObject warningPrefab;      // ground warning prefab

    [Header("Laser Settings")]
    public float startRadius = 0.1f;      // starting width (X/Z scale)
    public float finalRadius = 5f;        // target width (X/Z scale)
    public float chargeDuration = 3f;     // time before laser starts growing
    public float growDuration = 0.75f;    // time it takes to grow to full size
    public float activeDuration = 3f;     // time laser stays active after fully grown

    [Header("Warning")]
    public float warningRadius = 5f;
    public float warningDuration = 3f;

    [Header("Growth Curve")]
    public AnimationCurve growthCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

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

        // 2) Charge-up phase
        yield return new WaitForSeconds(chargeDuration);

        // 3) Activate laser at small size
        if (laserCylinder != null)
        {
            laserCylinder.SetActive(true);

            Vector3 scale = laserCylinder.transform.localScale;
            scale.x = startRadius;
            scale.z = startRadius;
            laserCylinder.transform.localScale = scale;
        }

        // 4) Smoothly grow laser
        float t = 0f;
        while (t < growDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / growDuration);
            float curveValue = growthCurve.Evaluate(normalized);

            float radius = Mathf.Lerp(startRadius, finalRadius, curveValue);

            if (laserCylinder != null)
            {
                Vector3 scale = laserCylinder.transform.localScale;
                scale.x = radius;
                scale.z = radius;
                laserCylinder.transform.localScale = scale;
            }

            yield return null;
        }

        // Ensure final size
        if (laserCylinder != null)
        {
            Vector3 scale = laserCylinder.transform.localScale;
            scale.x = finalRadius;
            scale.z = finalRadius;
            laserCylinder.transform.localScale = scale;
        }

        // 5) Laser active time
        yield return new WaitForSeconds(activeDuration);

        // 6) Cleanup
        if (laserCylinder != null)
            Destroy(laserCylinder);

        Destroy(gameObject);
    }

    private Vector3 GetGroundPosition(Vector3 origin)
    {
        if (Physics.Raycast(
            origin + Vector3.up * 10f,
            Vector3.down,
            out RaycastHit hit,
            50f,
            LayerMask.GetMask("Ground")))
        {
            return hit.point;
        }

        return origin;
    }
}
