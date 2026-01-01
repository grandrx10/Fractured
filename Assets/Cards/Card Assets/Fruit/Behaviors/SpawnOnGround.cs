using UnityEngine;

public class SpawnOnGround : MonoBehaviour
{
    [Header("Settings")]
    public GameObject groundPrefab;        // Optional prefab to spawn at ground beneath
    public GameObject collisionPrefab;     // Optional prefab to spawn at collision point
    public LayerMask groundLayer;          // Layer(s) to raycast against for ground
    public float maxRayDistance = 10f;     // Max distance to check for ground
    public float groundSpawnOffsetY = 0f;  // Optional Y offset above ground

    private void OnCollisionEnter(Collision collision)
    {
        // Spawn prefab at collision point (optional)
        if (collisionPrefab != null)
        {
            Instantiate(collisionPrefab, transform.position, Quaternion.identity);
        }

        // Raycast downwards to spawn prefab at ground (optional)
        if (groundPrefab != null)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxRayDistance, groundLayer))
            {
                Vector3 spawnPos = hit.point + Vector3.up * groundSpawnOffsetY;
                Instantiate(groundPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("SpawnOnGround: No ground found beneath object.");
            }
        }

        // Destroy this object after spawning
        Destroy(gameObject);
    }
}
