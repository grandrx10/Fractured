using UnityEngine;

public class RecurseOnHit : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab;
    public int spawnCount = 4;
    public float spawnRadius = 0.5f;
    public float spawnHeightOffset = 0.5f;

    [Header("Force Settings")]
    public float upwardVelocity = 6f;
    public float outwardVelocity = 4f;

    private bool hasTriggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasTriggered)
            return;

        hasTriggered = true;

        Vector3 center = transform.position + Vector3.up * spawnHeightOffset;

        float angleStep = 360f / spawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;

            // Evenly spaced direction on XZ plane
            Vector3 outwardDir = new Vector3(
                Mathf.Cos(angle),
                0f,
                Mathf.Sin(angle)
            ).normalized;

            Vector3 spawnPos = center + outwardDir * spawnRadius;

            GameObject spawned = Instantiate(prefab, spawnPos, Quaternion.identity);

            Rigidbody rb = spawned.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 velocity =
                    outwardDir * outwardVelocity +
                    Vector3.up * upwardVelocity;

                rb.linearVelocity = velocity;
            }
        }

        Destroy(gameObject);
    }
}
