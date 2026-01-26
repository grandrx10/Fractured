using UnityEngine;
using Game;

public class PrefabSwapOnLoad : MonoBehaviour
{
    [Header("Allowed Prefabs")]
    [Tooltip("List of prefabs this object is allowed to become")]
    public GameObject[] possiblePrefabs;

    private void Start()
    {
        if (GlobalState.instance == null)
            return;

        string key = $"PREFAB_{gameObject.name}";

        if (!GlobalState.instance.TryGetStr(key, out string prefabName))
            return;

        foreach (GameObject prefab in possiblePrefabs)
        {
            if (prefab != null && prefab.name == prefabName)
            {
                SwapToPrefab(prefab);
                return;
            }
        }

        Debug.LogWarning(
            $"PrefabSwapOnLoad: No matching prefab '{prefabName}' found for {gameObject.name}"
        );
    }

    private void SwapToPrefab(GameObject prefab)
    {
        Transform parent = transform.parent;
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        GameObject newObj = Instantiate(prefab, position, rotation, parent);
        newObj.name = gameObject.name; // preserve identity for future state lookups

        Debug.Log($"PrefabSwapOnLoad: {gameObject.name} swapped to {prefab.name}");

        Destroy(gameObject);
    }
}
