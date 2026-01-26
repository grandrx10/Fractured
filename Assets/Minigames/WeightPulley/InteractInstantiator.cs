using UnityEngine;
using Characters.Interactables;

public class InteractInstantiator : Interactable
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;
    public Transform spawnPoint;
    public bool destroyAfterInteract = false;

    public override void Interact(GameObject player)
    {
        if (!canInteract)
            return;

        if (prefabToSpawn == null || spawnPoint == null)
        {
            Debug.LogWarning("InteractInstantiator: Missing prefab or spawn point.");
            return;
        }

        Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        if (destroyAfterInteract)
        {
            Destroy(gameObject);
        }
    }
}
