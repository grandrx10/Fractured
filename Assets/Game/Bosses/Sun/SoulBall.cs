using Cards.Environments;
using UnityEngine;
using Characters;

public class SoulBall : MonoBehaviour
{
    [Header("Orbit Settings")]
    public float orbitRadius = 3f;
    public float orbitSpeed = 180f;
    public float orbitHeightOffset = 5f;
    public float orbitDurationDegrees = 450f;

    [Header("Launch Settings")]
    public float launchSpeed = 10f;
    public float targetRadius = 5f;

    [Header("Collision / Explosion")]
    public GameObject impactPrefab;

    private Rigidbody rb;
    private Transform bossTransform;
    private bool hasLaunched = false;
    private bool hasExploded = false;
    private float rotatedDegrees = 0f;
    private Vector3 launchTarget;

    public void SetBoss(Transform boss)
    {
        bossTransform = boss;
        transform.parent = bossTransform; // orbit locally
        transform.localPosition = Vector3.up * orbitHeightOffset + Vector3.right * orbitRadius;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        if (!hasLaunched)
        {
            // Local orbit around boss
            float deltaDegrees = orbitSpeed * Time.fixedDeltaTime;
            rotatedDegrees += deltaDegrees;
            float angleRad = rotatedDegrees * Mathf.Deg2Rad;
            Vector3 localOffset = new Vector3(
                Mathf.Cos(angleRad) * orbitRadius,
                Mathf.Sin(angleRad) * orbitRadius,
                0f
            );
            transform.localPosition = Vector3.up * orbitHeightOffset + localOffset;

            if (rotatedDegrees >= orbitDurationDegrees)
            {
                SetRandomTarget();
                hasLaunched = true;
            }
        }
        else if (!hasExploded)
        {
            // Move toward target
            Vector3 dir = (launchTarget - transform.position).normalized;
            float step = launchSpeed * Time.fixedDeltaTime;
            Vector3 newPos = transform.position + dir * step;

            if (Vector3.Distance(newPos, launchTarget) <= step)
            {
                rb.MovePosition(launchTarget);
                Explode();
            }
            else
            {
                rb.MovePosition(newPos);
            }
        }
    }

    private void SetRandomTarget()
    {
        // Unparent to move independently
        transform.parent = null;

        if (bossTransform == null) return;

        Vector3 playerPos = OpenWorldEnv.Current.PlayerPos;
        Vector3 randomOffset = Random.insideUnitSphere * targetRadius;
        randomOffset.y = 0f;

        launchTarget = new Vector3(
            playerPos.x + randomOffset.x,
            bossTransform.position.y,
            playerPos.z + randomOffset.z
        );
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (impactPrefab != null)
            Instantiate(impactPrefab, transform.position, Quaternion.identity);

        // Teleport boss to orb's position
        if (bossTransform != null)
            bossTransform.position = transform.position;

        Destroy(gameObject);
    }
}
