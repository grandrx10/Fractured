using UnityEngine;
using Game.Bosses.Projectiles;

public class FloorCutaway : MonoBehaviour
{
    [Header("Prefab Settings")]
    [SerializeField] private GameObject replacementPrefab;
    [SerializeField] private Vector2 prefabSize = new Vector2(1f, 1f);

    [Header("Physics")]
    [SerializeField] private float upwardForce = 6f;
    [SerializeField] private float outwardForce = 2f;
    [SerializeField] private ForceMode forceMode = ForceMode.Impulse;

    [Header("Audio")]
    [SerializeField] private AudioClip cutawaySound;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;
    [SerializeField] private bool randomizePitch = true;

    private Collider floorCollider;

    void Awake()
    {
        floorCollider = GetComponent<Collider>();
        if (floorCollider == null)
        {
            Debug.LogError("FloorCutaway requires a Collider on the floor object!");
        }
    }

    public void TriggerCutaway()
    {
        if (floorCollider == null || replacementPrefab == null)
            return;

        Bounds bounds = floorCollider.bounds;

        int countX = Mathf.CeilToInt(bounds.size.x / prefabSize.x);
        int countZ = Mathf.CeilToInt(bounds.size.z / prefabSize.y);

        Vector3 startPos = bounds.min + new Vector3(
            prefabSize.x * 0.5f,
            0f,
            prefabSize.y * 0.5f
        );

        Vector3 center = bounds.center;

        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                Vector3 spawnPos = startPos + new Vector3(
                    x * prefabSize.x,
                    0f,
                    z * prefabSize.y
                );

                GameObject piece = Instantiate(
                    replacementPrefab,
                    spawnPos,
                    Quaternion.identity,
                    transform.parent
                );

                if (piece.TryGetComponent<Rigidbody>(out var rb))
                {
                    Vector3 outwardDir = (spawnPos - center).normalized;
                    Vector3 force =
                        Vector3.up * upwardForce +
                        outwardDir * outwardForce;

                    rb.AddForce(force, forceMode);
                }
            }
        }

        // Play cutaway sound at floor center
        if (cutawaySound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(
                cutawaySound,
                Vector3.zero,   // position irrelevant for 2D
                volume,
                randomizePitch,
                spatialBlend: 0f
            );
        }


        gameObject.SetActive(false);
    }
}
