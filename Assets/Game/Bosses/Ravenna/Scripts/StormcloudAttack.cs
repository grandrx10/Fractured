using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Bosses;

[CreateAssetMenu(menuName = "BossAttacks/Ravenna/StormCloudAttack")]
public class StormCloudAttack : BossAttack
{
    [Header("Storm Cloud Settings")]
    public GameObject stormCloudPrefab;     // Prefab of the moving storm cloud
    public float spawnInterval = 2f;        // Time between cloud spawns
    public Vector2 lateralOffsetRange = new Vector2(-3f, 3f); // Left/right offset from spawn point

    [Header("Spawn Points")]
    public string[] spawnPointNames;         // Named points from boss to spawn at

    private float _spawnTimer;
    private Transform _currentPoint;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);

        _spawnTimer = 0f;

        // Pick a random spawn point from the list
        if (spawnPointNames != null && spawnPointNames.Length > 0)
        {
            Boss bossComponent = boss.GetComponent<Boss>();
            if (bossComponent != null)
            {
                string pointName = spawnPointNames[Random.Range(0, spawnPointNames.Length)];
                _currentPoint = bossComponent.GetPointTransform(pointName);

                Debug.Log(_currentPoint);
            }
        }

        Debug.Log("STarted the attack?");
    }

    public override void Tick(GameObject boss)
    {
        if (!isActive || stormCloudPrefab == null || _currentPoint == null)
            return;

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= spawnInterval)
        {
            _spawnTimer -= spawnInterval;
            SpawnStormCloud();
        }
    }

    private void SpawnStormCloud()
    {
        if (_currentPoint == null) return;

        // Pick a random lateral offset along the local X-axis of the point
        float offsetX = Random.Range(lateralOffsetRange.x, lateralOffsetRange.y);
        Vector3 spawnPos = _currentPoint.position + _currentPoint.right * offsetX;

        // Spawn the storm cloud facing the forward direction of the point
        Quaternion spawnRot = _currentPoint.rotation;

        GameObject cloud = GameObject.Instantiate(stormCloudPrefab, spawnPos, spawnRot);

        // Optional: parent to boss for hierarchy cleanliness
        cloud.transform.SetParent(_currentPoint.parent, worldPositionStays: true);
    }
}
