using System.Collections;
using UnityEngine;

public class DisassembleExplosion : MonoBehaviour
{
    [Header("Explosion Timing")]
    public float explodeDelay = 3f;
    public float destroyDelay = 3f;

    [Header("Explosion Force")]
    public float explosionForce = 12f;
    public float upwardForce = 3f;
    public float randomTorque = 6f;

    [Header("References")]
    [Tooltip("Parent whose children will be disassembled. If null, uses this transform.")]
    public Transform explodingParent;

    [Tooltip("Prefab spawned at the parent position when explosion happens")]
    public GameObject explosionPrefab;

    private void Start()
    {
        if (explodingParent == null)
            explodingParent = transform;

        StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        yield return new WaitForSeconds(explodeDelay);

        Vector3 origin = explodingParent.position;

        // Spawn explosion prefab
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, origin, Quaternion.identity);
        }

        // Detach and explode children
        foreach (Transform child in explodingParent)
        {
            child.SetParent(null);

            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb == null)
                rb = child.gameObject.AddComponent<Rigidbody>();

            Vector3 dir = (child.position - origin).normalized;
            rb.linearVelocity = dir * explosionForce + Vector3.up * upwardForce;

            Vector3 torque = Random.insideUnitSphere * randomTorque;
            rb.AddTorque(torque, ForceMode.Impulse);

            Destroy(child.gameObject, destroyDelay);
        }

        // Optionally destroy the parent itself
        Destroy(explodingParent.gameObject);
    }
}
