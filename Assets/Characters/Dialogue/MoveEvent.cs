using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Dialogue
{
    public class MoveEvent : DialogueEvent
    {
        [Header("Movement Settings")]
        public GameObject objectToMove;                 // Object being moved
        public List<Transform> targetLocations;         // Path points (in order)
        public float movementSpeed = 2f;                // Units per second

        [Header("Rotation Settings")]
        public bool faceMovementDirection = true;       // Rotate while moving
        public float rotationSpeed = 10f;               // Rotation speed
        public Transform endLookingAt;                  // Optional final look-at

        public override void Execute()
        {
            if (objectToMove == null || targetLocations == null || targetLocations.Count == 0)
            {
                Debug.LogError("MoveEvent missing objectToMove or targetLocations.");
                return;
            }

            DialogueManager.Instance.StartCoroutine(MovePathCoroutine());
        }

        private IEnumerator MovePathCoroutine()
        {
            foreach (Transform target in targetLocations)
            {
                if (target == null)
                    continue;

                yield return MoveToTargetCoroutine(target);
            }

            // Final rotation after completing the path
            if (endLookingAt != null)
            {
                Vector3 lookDir = endLookingAt.position - objectToMove.transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.001f)
                {
                    objectToMove.transform.rotation = Quaternion.LookRotation(lookDir);
                }
            }
        }

        private IEnumerator MoveToTargetCoroutine(Transform target)
        {
            Vector3 endPos = target.position;

            while (Vector3.Distance(objectToMove.transform.position, endPos) > 0.01f)
            {
                // Move
                objectToMove.transform.position = Vector3.MoveTowards(
                    objectToMove.transform.position,
                    endPos,
                    movementSpeed * Time.deltaTime
                );

                // Rotate to face movement direction (XZ plane only)
                if (faceMovementDirection)
                {
                    Vector3 direction = endPos - objectToMove.transform.position;
                    direction.y = 0f;

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

            // Snap exactly to target
            objectToMove.transform.position = endPos;
        }
    }
}
