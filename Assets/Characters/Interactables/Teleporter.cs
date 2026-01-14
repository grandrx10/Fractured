using System.Collections;
using System.Collections.Generic;
using Characters.Interactables;
using UnityEngine;

public class Teleporter : Interactable
{
    [Tooltip("Target transforms (must match object count)")]
    public Transform targetPosition;

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
        if (_isTeleporting)
            return;

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
        GlobalWorldManager.Instance.CurrentEnvironment.player.GetComponent<Rigidbody>().MovePosition(targetPosition.position);

        // Wait before fade IN
        yield return new WaitForSeconds(unfadeDelay);

        // Fade IN
        yield return GlobalWorldManager.Instance.StartCoroutine(
            GlobalWorldManager.Instance.Fade(reverse: true, duration: fadeDuration)
        );

        _isTeleporting = false;
    }
}
