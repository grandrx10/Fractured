using Cards.Environments;
using UnityEngine;

public class PlatformSplitter : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject platformPrefab;
    public bool disableOnUse = true;

    private void Awake()
    {
        GlobalWorldManager.OnLoadNewScene += Init;
    }

    private void Init(CardEnv _)
    {
        GlobalWorldManager.OnLoadNewScene -= Init;
        if (platformPrefab == null)
        {
            Debug.LogError("PlatformSplitter: No prefab assigned.");
            return;
        }

        Renderer sourceRenderer = GetComponent<Renderer>();
        if (sourceRenderer == null)
        {
            Debug.LogError("PlatformSplitter: GameObject has no Renderer.");
            return;
        }

        Renderer prefabRenderer = platformPrefab.GetComponentInChildren<Renderer>();
        if (prefabRenderer == null)
        {
            Debug.LogError("PlatformSplitter: Prefab has no Renderer.");
            return;
        }

        // Size of the original platform (XZ only)
        Vector3 sourceSize = sourceRenderer.bounds.size;

        // Size of the prefab tile (XZ only)
        Vector3 prefabSize = prefabRenderer.bounds.size;

        int countX = Mathf.CeilToInt(sourceSize.x / prefabSize.x);
        int countZ = Mathf.CeilToInt(sourceSize.z / prefabSize.z);

        Vector3 startCorner =
            sourceRenderer.bounds.center
            - new Vector3(sourceSize.x / 2f, 0f, sourceSize.z / 2f)
            + new Vector3(prefabSize.x / 2f, 0f, prefabSize.z / 2f);

        Transform parent = transform.parent;

        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                Vector3 position = startCorner + new Vector3(
                    x * prefabSize.x,
                    0f,
                    z * prefabSize.z
                );

                GameObject tile = Instantiate(
                    platformPrefab,
                    position,
                    transform.rotation,
                    parent
                );
            }
        }

        // Disable the original platform
        sourceRenderer.enabled = false;
        if (disableOnUse)
        {
            gameObject.SetActive(false);
        }
        
    }
}
