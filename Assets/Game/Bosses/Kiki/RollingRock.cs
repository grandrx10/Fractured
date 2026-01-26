using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingRock : MonoBehaviour
{
    [Header("Movement Settings")]
    public List<Transform> points = new List<Transform>();
    public float maxSpeed = 5f;
    public float acceleration = 10f;
    public bool autoStart = true;
    public float stopThreshold = 0.05f;

    [Header("Roll Through Settings")]
public float approachDistance = 40f;
public float rollThroughDistance = 80f;


    private int currentPointIndex = 0;
    private bool isMoving = false;
    private bool isOverridingPath = false;
    private Coroutine moveRoutine;
    private float currentSpeed = 0f;

    private void Start()
    {
        if (autoStart && points.Count > 0)
            StartRolling();
    }

    public void StartRolling()
    {
        if (points.Count == 0 || isMoving) return;

        isMoving = true;
        moveRoutine = StartCoroutine(MoveAlongPoints());
    }

    public void StopRolling()
    {
        if (!isMoving) return;

        isMoving = false;
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);
    }

    /* ===================== NORMAL PATH ===================== */

    private IEnumerator MoveAlongPoints()
    {
        while (true)
        {
            if (points.Count == 0) yield break;
            if (isOverridingPath)
            {
                yield return null;
                continue;
            }

            Transform targetPoint = points[currentPointIndex];
            currentSpeed = 0f;

            Vector3 targetPos = targetPoint.position;
            targetPos.y = transform.position.y;

            while (!isOverridingPath &&
                   Vector3.Distance(transform.position, targetPos) > stopThreshold)
            {
                currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

                Vector3 dir = (targetPos - transform.position);
                dir.y = 0f;
                dir.Normalize();

                transform.position += dir * currentSpeed * Time.deltaTime;
                yield return null;
            }

            if (!isOverridingPath)
            {
                transform.position = targetPos;
                currentPointIndex = (currentPointIndex + 1) % points.Count;
            }

            yield return null;
        }
    }

    /* ===================== ROLL THROUGH ===================== */

    public void RollThroughDoor(Transform doorTransform)
    {
        if (isOverridingPath || doorTransform == null || points.Count == 0)
            return;

        StartCoroutine(RollThroughRoutine(doorTransform));
    }

    private IEnumerator RollThroughRoutine(Transform door)
{
    isOverridingPath = true;
    currentSpeed = 0f;

    Vector3 approachPos = door.position + door.right * approachDistance;
    approachPos.y = transform.position.y;

    Vector3 exitPos = approachPos - door.right * rollThroughDistance;
    exitPos.y = transform.position.y;

    // Move to approach point (40 units away)
    yield return MoveToPosition(approachPos);

    // Roll THROUGH the door
    yield return MoveToPosition(exitPos);

    // Resume pathing
    currentPointIndex = GetNearestPointIndex();
    isOverridingPath = false;
}


    private IEnumerator MoveToPosition(Vector3 target)
    {
        target.y = transform.position.y;
        currentSpeed = 0f;

        while (Vector3.Distance(transform.position, target) > stopThreshold)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

            Vector3 dir = (target - transform.position);
            dir.y = 0f;
            dir.Normalize();

            transform.position += dir * currentSpeed * Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    private int GetNearestPointIndex()
    {
        int nearest = 0;
        float bestDist = float.MaxValue;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = points[i].position;
            p.y = transform.position.y;

            float d = Vector3.Distance(transform.position, p);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = i;
            }
        }

        return nearest;
    }
}
