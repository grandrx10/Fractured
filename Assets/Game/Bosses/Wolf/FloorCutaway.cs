using UnityEngine;
using Game.Bosses.Projectiles;
public class FloorCutaway : MonoBehaviour
{
    [Header("Prefab Settings")]
    [SerializeField] private GameObject replacementPrefab; // The prefab to spawn
    [SerializeField] private Vector2 prefabSize = new Vector2(1f, 1f); // Size of each prefab (X = width, Z = depth)

    private Collider floorCollider;

    void Awake()
    {
        floorCollider = GetComponent<Collider>();
        if (floorCollider == null)
        {
            Debug.LogError("FloorCutaway requires a Collider on the floor object!");
        }
    }

    /// <summary>
    /// Call this function to replace the floor with prefabs
    /// </summary>
    public void TriggerCutaway()
    {
        if (floorCollider == null || replacementPrefab == null)
            return;

        // Get floor bounds
        Bounds bounds = floorCollider.bounds;

        // Calculate how many prefabs fit along X and Z
        int countX = Mathf.CeilToInt(bounds.size.x / prefabSize.x);
        int countZ = Mathf.CeilToInt(bounds.size.z / prefabSize.y);

        // Bottom-left corner as starting point
        Vector3 startPos = bounds.min + new Vector3(prefabSize.x / 2f, 0f, prefabSize.y / 2f);

        // Spawn prefabs in a grid
        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                Vector3 spawnPos = startPos + new Vector3(x * prefabSize.x, 0f, z * prefabSize.y);
                Instantiate(replacementPrefab, spawnPos, Quaternion.identity, transform.parent);
            }
        }

        // Disable or destroy the original floor
        gameObject.SetActive(false); // or Destroy(gameObject);
    }
}
