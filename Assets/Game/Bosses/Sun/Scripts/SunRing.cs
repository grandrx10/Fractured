using Cards.Environments;
using UnityEngine;
using Characters; // For PlayerSingleton

public class SunRing : MonoBehaviour
{
    [Header("Orbit Settings")]
    public float orbitRadius = 3f;
    public float orbitSpeed = 180f;
    public float orbitHeightOffset = 5f;
    private float rotatedDegrees = 0f;

    [Header("Launch Settings")]
    public float launchSpeed = 10f;
    public float targetRadius = 5f; // used only to randomize direction

    [Header("Collision")]
    public GameObject impactPrefab;

    private Rigidbody rb;
    private bool hasLaunched = false;
    private bool hasImpacted = false;
    private Vector3 launchDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Start at the right edge of the orbit in local space
        transform.localPosition = Vector3.up * orbitHeightOffset + Vector3.right * orbitRadius;
    }

    void FixedUpdate()
    {
        if (!hasLaunched)
        {
            // Orbit logic in local space
            float deltaDegrees = orbitSpeed * Time.fixedDeltaTime;
            rotatedDegrees += deltaDegrees;

            float angleRad = rotatedDegrees * Mathf.Deg2Rad;

            Vector3 localOffset = new Vector3(
                Mathf.Cos(angleRad) * orbitRadius,
                Mathf.Sin(angleRad) * orbitRadius,
                0f
            );

            transform.localPosition = Vector3.up * orbitHeightOffset + localOffset;

            if (rotatedDegrees >= 450f)
            {
                hasLaunched = true;
                SetLaunchDirection();
            }
        }
        else if (!hasImpacted)
        {
            Vector3 newPos = transform.position + launchDirection * launchSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }
    }

    private void SetLaunchDirection()
    {
        // Unparent to move independently
        transform.parent = null;

        Vector3 playerPos = OpenWorldEnv.Current.PlayerPos;

        Vector3 randomOffset = Random.insideUnitSphere * targetRadius;
        Vector3 targetPoint = playerPos + randomOffset;

        launchDirection = (targetPoint - transform.position).normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasImpacted)
            return;

        hasImpacted = true;
        Explode();
    }

    private void Explode()
    {
        if (impactPrefab != null)
            Instantiate(impactPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
