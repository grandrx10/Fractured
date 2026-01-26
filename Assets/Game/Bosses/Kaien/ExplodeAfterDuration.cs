using UnityEngine;

public class ExplodeAfterDuration : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float duration = 2f;            // Time before replacement
    public GameObject replacementPrefab;   // Prefab to spawn on "explosion"

    private void Start()
    {
        // Start the coroutine to handle timed replacement
        StartCoroutine(ExplodeRoutine());
    }

    private System.Collections.IEnumerator ExplodeRoutine()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Spawn the replacement prefab at the current position & rotation
        if (replacementPrefab != null)
        {
            Instantiate(replacementPrefab, transform.position, transform.rotation);
        }

        // Destroy this game object
        Destroy(gameObject);
    }
}
