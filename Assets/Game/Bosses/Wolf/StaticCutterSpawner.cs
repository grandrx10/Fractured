using UnityEngine;
using Cards.Card_Assets.RPS.Behaviors;

public class StaticCutterSpawner : MonoBehaviour
{
    [Header("Cutter")]
    public Cutter cutterPrefab;

    private bool hasSpawned;

    private void OnTriggerEnter(Collider other)
    {
        if (hasSpawned)
            return;

        // Ground hit → destroy without spawning
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
            return;
        }

        ICuttable cuttable = other.GetComponent<ICuttable>();
        if (cuttable == null)
            return;

        hasSpawned = true;

        Instantiate(
            cutterPrefab,
            transform.position,
            transform.rotation
        );

        Destroy(gameObject);
    }

}
