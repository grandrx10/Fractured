using UnityEngine;

public class StraightProton : MonoBehaviour
{
    [Header("Flight Settings")]
    public float speed = 15f;           // Units per second
    public float maxDistance = 20f;     // Maximum distance to travel

    [Header("Collision")]
    public GameObject impactPrefab;     // Effect prefab on impact

    private Vector3 startPosition;
    private bool hasImpacted = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (hasImpacted) return;

        // Move forward in local Z direction
        Vector3 move = transform.forward * speed * Time.fixedDeltaTime;
        transform.position += move;

        // Check if max distance reached
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasImpacted) return;

        // Optional: ignore collisions with certain layers or tags
        // if(other.CompareTag("Player")) return;

        Explode();
    }

    private void Explode()
    {
        hasImpacted = true;

        if (impactPrefab != null)
        {
            Instantiate(impactPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
