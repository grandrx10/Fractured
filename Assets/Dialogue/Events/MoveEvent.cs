using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "MoveEvent", menuName = "DialogueEvents/MoveEvent")]
public class MoveEvent : DialogueEvent
{
    public Transform targetTransform; // drag in the scene
    public Vector3 targetPosition;    // destination
    public float duration = 1f;

    public override void Execute()
    {
        if (targetTransform != null)
        {
            DialogueManager.Instance.StartCoroutine(MoveCoroutine());
        }
    }

    private IEnumerator MoveCoroutine()
    {
        Vector3 startPosition = targetTransform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            targetTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        targetTransform.position = targetPosition;
    }
}
