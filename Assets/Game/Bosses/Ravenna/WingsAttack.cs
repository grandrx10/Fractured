using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Bosses;
using Cards.Environments;

[CreateAssetMenu(menuName = "BossAttacks/Ravenna/WingsAttack")]
public class WingsAttack : BossAttack
{
    [Header("Wings Attack Settings")]
    public GameObject spawnPrefab;       // Prefab to spawn at each point
    public string[] pointNames;          // Named points to spawn at
    public float spawnInterval = 1f;     // Time between spawns while active

    [Header("Pulse Settings")]
    public float onDuration = 3f;        // Seconds spawning is on
    public float offDuration = 3f;       // Seconds spawning is off

    [Header("Move Settings")]
    public string[] movePoints;          // List of points to randomly pick from
    public float moveDuration = 1f;      // Seconds to lerp to target point

    private float _spawnTimer;
    private float _pulseTimer;
    private bool _spawningEnabled;
    private bool _moving;
    private bool _waitingForNextCycle;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);
        _spawnTimer = 0f;
        _pulseTimer = 0f;
        _spawningEnabled = false;
        _moving = false;
        _waitingForNextCycle = false;

        // Start by moving to a random point
        MoveToRandomPoint(boss);

        // Optional: face player immediately
        NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
        if (npcCommands != null && OpenWorldEnv.Current != null)
        {
            npcCommands.LookingAt = OpenWorldEnv.Current.PlayerTransform;
        }
    }

    public override void Tick(GameObject boss)
    {
        if (!isActive || spawnPrefab == null || pointNames.Length == 0)
            return;

        // Don't do anything while moving or waiting
        if (_moving || _waitingForNextCycle)
            return;

        // --- Handle spawning ---
        if (_spawningEnabled)
        {
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= spawnInterval)
            {
                _spawnTimer -= spawnInterval;
                SpawnAtPoints(boss);
            }

            // --- Check if shooting duration is complete ---
            _pulseTimer += Time.deltaTime;
            if (_pulseTimer >= onDuration)
            {
                // Turn off spawning and start off phase
                _spawningEnabled = false;
                _pulseTimer = 0f;
                Boss bossComponent = boss.GetComponent<Boss>();
                if (bossComponent != null)
                {
                    bossComponent.StartCoroutine(OffPhaseCoroutine(boss));
                }
            }
        }
    }

    private void MoveToRandomPoint(GameObject boss)
    {
        if (movePoints == null || movePoints.Length == 0 || _moving)
            return;

        Boss bossComponent = boss.GetComponent<Boss>();
        if (bossComponent == null)
            return;

        string randomPointName = movePoints[Random.Range(0, movePoints.Length)];
        Transform target = bossComponent.GetPointTransform(randomPointName);
        if (target != null)
        {
            bossComponent.StartCoroutine(MoveToPointCoroutine(boss, target.position, moveDuration));
        }
    }

    private IEnumerator MoveToPointCoroutine(GameObject boss, Vector3 targetPos, float duration)
    {
        _moving = true;
        float elapsed = 0f;
        Vector3 startPos = boss.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            boss.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        boss.transform.position = targetPos;
        _moving = false;
        
        // After moving, start shooting
        _spawningEnabled = true;
        _pulseTimer = 0f;
        _spawnTimer = 0f;
    }

    private IEnumerator OffPhaseCoroutine(GameObject boss)
    {
        _waitingForNextCycle = true;
        
        // Wait for off duration (pause phase)
        yield return new WaitForSeconds(offDuration);
        
        _waitingForNextCycle = false;
        
        // Move to a new random point, which will then trigger shooting again
        MoveToRandomPoint(boss);
    }

    private void SpawnAtPoints(GameObject boss)
    {
        Boss bossComponent = boss.GetComponent<Boss>();
        if (bossComponent == null)
            return;

        foreach (string pointName in pointNames)
        {
            Transform t = bossComponent.GetPointTransform(pointName);
            if (t == null)
                continue;

            // Random tilt around X-axis (-15° to 15°) for ZY plane
            float randomAngle = Random.Range(-15f, 15f);
            Quaternion spawnRot = t.rotation * Quaternion.Euler(randomAngle, 0f, 0f);

            GameObject obj = GameObject.Instantiate(spawnPrefab, t.position, spawnRot);
        }
    }
}