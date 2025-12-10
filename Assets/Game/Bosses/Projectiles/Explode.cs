using UnityEngine;

public class Explode : MonoBehaviour
{
    [Header("Explosion Prefab")]
    public GameObject explosionPrefab;

    [Header("Explosion Settings")]
    public bool explodeOnTouch = false;     // If true → trigger explosion on any trigger enter

    // Public call to explode manually
    public void ExplodeNow()
    {
        // Spawn explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Destroy this object after explosion
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
{
    if (!explodeOnTouch) return;

    // Debug.Log($"Trigger entered by: {other.gameObject.name} on layer: {LayerMask.LayerToName(other.gameObject.layer)}");
    
    // Only explode if the object we touched is on the "Player" layer
    if (other.gameObject.layer != LayerMask.NameToLayer("Player") && 
    other.gameObject.layer != LayerMask.NameToLayer("Ground")) return;

    // Debug.Log("Touched Player");

    // Prevent multiple explosions
    explodeOnTouch = false;

    ExplodeNow();
}

}
