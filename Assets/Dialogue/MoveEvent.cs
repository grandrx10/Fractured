using UnityEngine;
using System.Collections;

public class MoveEvent : DialogueEvent
{
    [Header("Movement Settings")]
    public GameObject objectToMove;      // The object being moved
    public Transform targetLocation;     // The destination transform
    public float duration = 1f;          // Time to complete the movement

    [Header("Rotation Settings")]
    public bool faceMovementDirection = true;   // Should the object rotate while moving
    public float rotationSpeed = 10f;           // How fast the object rotates towards movement direction
    public Transform endLookingAt;              // Optional transform to look at after movement ends

    public override void Execute()
    {
        if (objectToMove == null || targetLocation == null)
        {
            Debug.LogError("MoveEvent missing objectToMove or targetLocation.");
            return;
        }

        DialogueManager.Instance.StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Vector3 startPos = objectToMove.transform.position;
        Vector3 endPos = targetLocation.position;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Move position
            objectToMove.transform.position = Vector3.Lerp(startPos, endPos, t);

            // Rotate to face movement direction (XZ plane only)
            if (faceMovementDirection)
            {
                Vector3 direction = endPos - objectToMove.transform.position;
                direction.y = 0f; // Only rotate on XZ plane

                if (direction.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(direction);
                    objectToMove.transform.rotation = Quaternion.Slerp(
                        objectToMove.transform.rotation,
                        targetRot,
                        rotationSpeed * Time.deltaTime
                    );
                }
            }

            yield return null;
        }

        // Final snap to position
        objectToMove.transform.position = endPos;

        // Rotate at the end if endLookingAt is set (XZ plane only)
        if (endLookingAt != null)
        {
            Vector3 lookDir = endLookingAt.position - objectToMove.transform.position;
            lookDir.y = 0f; // Only rotate on XZ plane

            if (lookDir.sqrMagnitude > 0.001f)
            {
                objectToMove.transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }
}
