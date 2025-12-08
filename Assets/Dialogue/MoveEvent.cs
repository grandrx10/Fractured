using UnityEngine;
using System.Collections;

public class MoveEvent : DialogueEvent
{
    [Header("Movement Settings")]
    public GameObject objectToMove;     // The object being moved
    public Transform targetLocation;    // The destination transform
    public float duration = 1f;         // Time to complete the movement

    public override void Execute()
    {
        if (objectToMove == null || targetLocation == null)
        {
            Debug.LogError("MoveEvent missing objectToMove or targetLocation.");
            return;
        }

        // Start the coroutine on the DialogueManager so it survives inside ScriptableObject events
        DialogueManager.Instance.StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Vector3 startPos = objectToMove.transform.position;
        Vector3 endPos   = targetLocation.position;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            objectToMove.transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        // Final snap
        objectToMove.transform.position = endPos;
    }
}
