using UnityEngine;
using Cards.Card_Assets.Fishing.B;

public class FishingRod : MonoBehaviour
{
    public GameObject hookPrefab;
    public float throwSpeed = 15f;
    public Transform hookSpawnPoint;
    public FishingBehaviour behaviour; // reference to behaviour

    private Vector3 lookDirection;
    private GameObject spawnedHook;

    public RhythmFishingBars fishingMinigame; // optional, can be set by FishingPool

    public void Initialize(Vector3 direction, FishingBehaviour behaviourRef)
    {
        lookDirection = direction.normalized;
        behaviour = behaviourRef;
        Throw();
    }

    public void Throw()
    {
        if (hookPrefab == null || hookSpawnPoint == null || spawnedHook != null) return;

        spawnedHook = Instantiate(
            hookPrefab,
            hookSpawnPoint.position,
            Quaternion.LookRotation(lookDirection, Vector3.up)
        );

        // Pass behaviour to hook
        FishingHook hookScript = spawnedHook.GetComponent<FishingHook>();
        if (hookScript != null)
        {
            hookScript.behaviour = behaviour;
        }

        Rigidbody rb = spawnedHook.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = lookDirection * throwSpeed;
            Debug.Log("Thrown out!");
        }
        else
        {
            Debug.LogWarning("Hook prefab has no Rigidbody");
        }
    }

    private void OnDestroy()
    {
        if (spawnedHook != null)
        {
            Destroy(spawnedHook);
        }
    }
}
