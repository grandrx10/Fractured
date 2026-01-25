using UnityEngine;
using System.Collections;
using Cards.Environments;
using Characters.Interactables;

public class DiscretePushBox : Interactable
{
    [Header("Push Settings")]
    public float pushDistance = 3f;
    public LayerMask groundLayer;
    public float lerpDuration = 1f;

    private bool isMoving = false;
    private Collider[] boxColliders;
    private Coroutine fallRoutine;

    private void Awake()
    {
        // Cache all colliders belonging to this box (including children)
        boxColliders = GetComponentsInChildren<Collider>();
    }

    private void Update()
    {
        // Passive falling if ground disappears
        if (isMoving)
            return;

        if (!HasGroundBelow())
        {
            fallRoutine ??= StartCoroutine(PassiveFall());
        }
    }

    public override void Interact(GameObject player)
    {
        if (!canInteract || isMoving)
            return;

        Vector3 playerPos = OpenWorldEnv.Current.PlayerPos;
        Vector3 boxPos = transform.position;

        // Direction from player to box (XZ only)
        Vector3 toBox = boxPos - playerPos;
        toBox.y = 0f;
        toBox.Normalize();

        Vector3 pushDir;

        // Determine cardinal direction
        if (Mathf.Abs(toBox.x) > Mathf.Abs(toBox.z))
            pushDir = toBox.x > 0 ? Vector3.right : Vector3.left;
        else
            pushDir = toBox.z > 0 ? Vector3.forward : Vector3.back;

        Vector3 targetPos = boxPos + pushDir * pushDistance;
        targetPos.y = boxPos.y;

        if (IsPathBlocked(boxPos, pushDir, pushDistance))
            return;

        StartCoroutine(PushAndFall(targetPos));
    }

    private bool IsPathBlocked(Vector3 origin, Vector3 direction, float distance)
    {
        // Wall check only
        return Physics.Raycast(origin, direction, distance, groundLayer);
    }

    private IEnumerator PushAndFall(Vector3 horizontalTarget)
    {
        isMoving = true;

        // Phase 1: horizontal push
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lerpDuration;
            transform.position = Vector3.Lerp(startPos, horizontalTarget, t);
            yield return null;
        }

        transform.position = horizontalTarget;

        // Phase 2: falling
        yield return StartCoroutine(ApplyDiscreteFall());

        isMoving = false;
    }

    private IEnumerator PassiveFall()
    {
        isMoving = true;

        yield return StartCoroutine(ApplyDiscreteFall());

        isMoving = false;
        fallRoutine = null;
    }

    private bool HasGroundBelow()
    {
        float boxHeight = 3f;
        float epsilon = 0.05f;

        Vector3 rayStart =
            transform.position + Vector3.down * (boxHeight / 2f - epsilon);

        float checkDistance = epsilon + 0.02f;

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, checkDistance, groundLayer))
        {
            // Ignore self-collisions
            foreach (var col in boxColliders)
            {
                if (hit.collider == col)
                    return false;
            }

            return true;
        }

        return false;
    }

    private IEnumerator ApplyDiscreteFall()
    {
        int maxFallSteps = 100;
        int steps = 0;

        float boxHeight = 3f;
        float epsilon = 0.05f;

        while (steps < maxFallSteps)
        {
            Vector3 currentPos = transform.position;

            Vector3 rayStart =
                currentPos + Vector3.down * (boxHeight / 2f - epsilon);

            float checkDistance = boxHeight + epsilon;

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, checkDistance, groundLayer))
            {
                bool hitSelf = false;
                foreach (var col in boxColliders)
                {
                    if (hit.collider == col)
                    {
                        hitSelf = true;
                        break;
                    }
                }

                if (!hitSelf)
                    break;
            }

            Vector3 targetPos = currentPos + Vector3.down * boxHeight;

            float elapsed = 0f;
            while (elapsed < lerpDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lerpDuration;
                transform.position = Vector3.Lerp(currentPos, targetPos, t);
                yield return null;
            }

            transform.position = targetPos;
            steps++;
        }
    }
}
