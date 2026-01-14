using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormCloud : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;      // Forward movement speed

    [Header("Spawn Settings")]
    public GameObject spawnPrefab;     // Prefab to spawn
    public float spawnInterval = 0.5f; // Time between spawns
    public Vector3 spawnAreaSize = new Vector3(5f, 0f, 3f); // X,Z rectangle for spawning

    private float _spawnTimer;

    void Update()
    {
        // --- Move the cloud forward ---
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // --- Spawn prefabs at intervals ---
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= spawnInterval)
        {
            _spawnTimer -= spawnInterval;
            SpawnPrefabAcrossCloud();
        }
    }

    private void SpawnPrefabAcrossCloud()
    {
        if (spawnPrefab == null) return;

        // Random point within XZ rectangle around cloud center
        float offsetX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float offsetZ = Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);

        Vector3 spawnPos = transform.position + new Vector3(offsetX, 0f, offsetZ);

        // Spawn prefab facing downward
        Quaternion spawnRot = Quaternion.Euler(90f, 0f, 0f); // facing down on Y-axis

        GameObject obj = Instantiate(spawnPrefab, spawnPos, spawnRot);

        // Optional: parent to cloud for organization
        obj.transform.SetParent(transform, worldPositionStays: true);
    }
}
