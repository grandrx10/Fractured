using System.Collections.Generic;
using UnityEngine;
using Cards.Card_Assets.RPS.Behaviors;

public class HeavySlashCollision : MonoBehaviour
{
    [Header("Cutter")]
    public Cutter cutterPrefab;

    // Track what we've already hit
    private readonly HashSet<ICuttable> hitTargets = new HashSet<ICuttable>();

    private void OnTriggerEnter(Collider other)
    {
        ICuttable cuttable = other.GetComponent<ICuttable>();
        if (cuttable == null)
            return;

        // Already spawned a cutter for this target
        if (hitTargets.Contains(cuttable))
            return;

        hitTargets.Add(cuttable);

        Instantiate(
            cutterPrefab,
            transform.position,
            transform.rotation
        );
    }
}
