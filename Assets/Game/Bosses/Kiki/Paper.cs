using UnityEngine;
using Characters.Interactables;
using System.Collections;

public class Paper : Interactable
{
    [HideInInspector] public KikiMinigameManager manager;
    [HideInInspector] public Transform spawnPoint;

    [Header("Growth Settings")]
    public GameObject growPrefab; // The object to duplicate
    public float growRadius = 5f;
    public float growInterval = 3f;
    [Range(0f, 1f)]
    public float growChance = 2f / 3f;

    private void Start()
    {
        if (growPrefab != null)
            StartCoroutine(GrowRoutine());
    }

    public override void Interact(GameObject player)
    {
        if (!canInteract) return;

        if (manager != null)
            manager.OnPaperCollected(this);

        Destroy(gameObject);
    }

    private IEnumerator GrowRoutine()
{
    while (true)
    {
        yield return new WaitForSeconds(growInterval);

        if (Random.value <= growChance)
        {
            // Random offset in XZ
            Vector2 randomXZ = Random.insideUnitCircle * growRadius;
            Vector3 offset = new Vector3(randomXZ.x, 0f, randomXZ.y);

            // Random Y rotation
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // Instantiate and parent
            GameObject newObj = Instantiate(growPrefab, transform.position + offset, randomRotation, transform);

            // Optional: preserve local Y of parent
            Vector3 pos = newObj.transform.localPosition;
            pos.y = 0f;
            newObj.transform.localPosition = pos;
        }
    }
}

}
