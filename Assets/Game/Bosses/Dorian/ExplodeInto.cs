using UnityEngine;

public class ExplodeInto : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject prefabToSpawn;   // Prefab to spawn
    public int count = 3;              // Number of prefabs
    public float radius = 1f;          // Distance from center

    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }

    private void Explode()
    {
        if (prefabToSpawn == null)
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2 / count; // spread in circle
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)); // outward direction
            Vector3 spawnPos = transform.position + direction * radius;

            // Rotate the spawned object to face outward
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            GameObject spawned = Instantiate(prefabToSpawn, spawnPos, rotation);
        }

        Destroy(gameObject);
    }
}
