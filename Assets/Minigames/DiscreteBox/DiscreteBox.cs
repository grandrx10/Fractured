using System;
using UnityEngine;
using System.Collections;
using Cards.Environments;
using Cards.PhysicalProperties;
using Characters.Interactables;
using Minigames;
using Utils;

public class DiscretePushBox : Interactable, IPuzzleResettable
{
    public string id;
    [Header("Push Settings")]
    public float pushDistance = 3f;
    private float _epsilon = 0.3f;
    public LayerMask groundLayer;
    public float lerpDuration = 1f;
    public bool keepMoving;
    public bool isMoving = false;
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

        Vector3 playerPos = player.transform.position;
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

        Push(pushDir);
    }

    private void Push(Vector3 pushDir)
    {
        Vector3 boxPos = transform.position;
        Vector3 targetPos = boxPos + pushDir * pushDistance;
        targetPos.y = boxPos.y;

        if (IsPathBlocked(boxPos, pushDir, pushDistance - _epsilon))
            return;

        StartCoroutine(PushAndFall(pushDir, targetPos));
    }

    private bool IsPathBlocked(Vector3 origin, Vector3 direction, float distance)
    {
        // Wall check only
        return Physics.Raycast(origin, direction, distance, groundLayer, QueryTriggerInteraction.Ignore);
    }
    
    bool _fell;
    
    private IEnumerator PushAndFall(Vector3 dir, Vector3 horizontalTarget)
    {
        isMoving = true;
        canInteract = false;
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
        _fell = false;
        // Phase 2: falling
        yield return StartCoroutine(ApplyDiscreteFall());
        isMoving = false;
        canInteract = true;
        if (keepMoving && !_fell) Push(dir);
    }

    private IEnumerator PassiveFall()
    {
        isMoving = true;
        canInteract = false;
        yield return StartCoroutine(ApplyDiscreteFall());

        isMoving = false;
        canInteract = true;
        fallRoutine = null;
    }

    private bool HasGroundBelow()
    {
        float boxHeight = pushDistance;

        Vector3 rayStart =
            transform.position + Vector3.down * (boxHeight / 2f - _epsilon);

        float checkDistance = _epsilon + 0.02f;

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, checkDistance, groundLayer, QueryTriggerInteraction.Ignore))
        {
            // Ignore self-collisions
            foreach (var col in boxColliders)
            {
                if (hit.collider == col)
                    return false;
            }
            //Debug.Log(hit.collider.name);
            return true;
        }

        return false;
    }

    private IEnumerator ApplyDiscreteFall()
    {
        int maxFallSteps = 100;
        int steps = 0;

        float boxHeight = pushDistance;

        while (steps < maxFallSteps)
        {
            Vector3 currentPos = transform.position;

            Vector3 rayStart =
                currentPos + Vector3.down * (boxHeight / 2f - _epsilon);

            float checkDistance = boxHeight/2 + _epsilon;

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, checkDistance, groundLayer, QueryTriggerInteraction.Ignore))
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

            _fell = true;
            transform.position = targetPos;
            steps++;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        var obj = PhysicsHelper.MainObj(other.collider);
        if (obj && obj.TryGetComponent(out PhysicalObject pObj))
        {
            Interact(obj);
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = isMoving ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale/2);
    }
#endif
    public void Reset(Pose parent)
    {
        StopAllCoroutines();
        GetComponent<Rigidbody>().isKinematic = true;
        isMoving = false;
        canInteract = true;
    }
}
