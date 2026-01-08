using System.Collections;
using System.Collections.Generic;
using Characters.Interactables;
using UnityEngine;

public class Teleporter : Interactable
{
    [Header("Teleport Objects")]
    [Tooltip("Objects to teleport")]
    public List<GameObject> objectsToTeleport = new();

    [Tooltip("Target transforms (must match object count)")]
    public List<Transform> targetPositions = new();

    [Header("Fade Settings")]
    [Tooltip("Fade-out duration (seconds)")]
    public float fadeDuration = 0.75f;

    [Tooltip("Delay after fade-out before teleport")]
    public float teleportDelay = 0.05f;

    [Tooltip("Delay after teleport before fade-in")]
    public float unfadeDelay = 0.15f;

    private bool _isTeleporting;

    public override void Interact(GameObject player)
    {
        if (!canInteract || _isTeleporting)
            return;

        if (objectsToTeleport.Count != targetPositions.Count)
        {
            Debug.LogError(
                $"Teleporter mismatch: {objectsToTeleport.Count} objects, {targetPositions.Count} targets",
                this
            );
            return;
        }

        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        _isTeleporting = true;

        // Fade OUT
        yield return GlobalWorldManager.Instance.StartCoroutine(
            GlobalWorldManager.Instance.Fade(reverse: false, duration: fadeDuration)
        );

        yield return new WaitForSeconds(teleportDelay);

        // Teleport objects
        for (int i = 0; i < objectsToTeleport.Count; i++)
        {
            if (!objectsToTeleport[i] || !targetPositions[i]) continue;

            objectsToTeleport[i].transform.SetPositionAndRotation(
                targetPositions[i].position,
                targetPositions[i].rotation
            );
        }

        // Wait before fade IN
        yield return new WaitForSeconds(unfadeDelay);

        // Fade IN
        yield return GlobalWorldManager.Instance.StartCoroutine(
            GlobalWorldManager.Instance.Fade(reverse: true, duration: fadeDuration)
        );

        _isTeleporting = false;
    }
}
