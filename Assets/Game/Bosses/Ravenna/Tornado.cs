using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards.Environments;

public class Tornado : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject projectilePrefab;
    public float spawnInterval = 0.08f;
    public int maxProjectiles = 50;

    [Header("Tornado Shape")]
    public float baseRadius = 0.5f; // Radius at the bottom
    public float topRadius = 3f; // Radius at the top
    public float tornadoHeight = 8f;
    public float spiralTightness = 2f; // How many rotations to complete going up

    [Header("Chaos Settings")]
    public float positionNoise = 0.3f; // Random position offset
    public float speedVariation = 0.2f; // Random speed variation per projectile
    
    [Header("Movement")]
    public float upwardSpeed = 4f; // How fast projectiles rise
    public float rotationSpeed = 360f; // Base rotation speed in degrees/sec
    public float tornadoMoveSpeed = 12f;
    public float delayBeforeMove = 2f;

    private class TornadoProjectile
    {
        public GameObject obj;
        public float startAngle;
        public float heightProgress; // 0 to 1, bottom to top
        public float speedMultiplier;
        public Vector3 randomOffset;
    }

    private readonly List<TornadoProjectile> _projectiles = new();
    private Vector3 _lockedTargetDir;
    private bool _isMoving;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
        StartCoroutine(StartMovementAfterDelay());
    }

    void Update()
    {
        UpdateTornadoProjectiles();

        if (_isMoving)
        {
            transform.position += _lockedTargetDir * tornadoMoveSpeed * Time.deltaTime;
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (_projectiles.Count < maxProjectiles)
        {
            SpawnProjectile();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnProjectile()
    {
        // Start at the bottom with a random angle
        float startAngle = Random.Range(0f, 360f);
        
        // Random offset for chaos
        Vector3 randomOffset = new Vector3(
            Random.Range(-positionNoise, positionNoise),
            Random.Range(-positionNoise * 0.5f, positionNoise * 0.5f),
            Random.Range(-positionNoise, positionNoise)
        );

        // Spawn at base
        float angleRad = startAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(angleRad) * baseRadius,
            0f,
            Mathf.Sin(angleRad) * baseRadius
        ) + randomOffset;

        Vector3 spawnPos = transform.position + offset;

        // Face tangent direction
        Vector3 tangent = new Vector3(-Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));
        Quaternion rot = Quaternion.LookRotation(tangent, Vector3.up);

        GameObject obj = Instantiate(projectilePrefab, spawnPos, rot);
        obj.transform.parent = gameObject.transform;

        TornadoProjectile tornado = new TornadoProjectile
        {
            obj = obj,
            startAngle = startAngle,
            heightProgress = 0f,
            speedMultiplier = 1f + Random.Range(-speedVariation, speedVariation),
            randomOffset = randomOffset
        };

        _projectiles.Add(tornado);
    }

    void UpdateTornadoProjectiles()
    {
        float deltaTime = Time.deltaTime;

        for (int i = _projectiles.Count - 1; i >= 0; i--)
        {
            TornadoProjectile proj = _projectiles[i];
            
            if (proj.obj == null)
            {
                _projectiles.RemoveAt(i);
                continue;
            }

            // Move upward
            proj.heightProgress += (upwardSpeed / tornadoHeight) * deltaTime * proj.speedMultiplier;

            // Loop back to bottom when reaching top
            if (proj.heightProgress >= 1f)
            {
                proj.heightProgress = 0f;
                proj.startAngle = Random.Range(0f, 360f); // New random angle
                proj.randomOffset = new Vector3(
                    Random.Range(-positionNoise, positionNoise),
                    Random.Range(-positionNoise * 0.5f, positionNoise * 0.5f),
                    Random.Range(-positionNoise, positionNoise)
                );
            }

            // Calculate spiral position
            float currentHeight = proj.heightProgress * tornadoHeight;
            
            // Interpolate radius based on height (wider at top)
            float currentRadius = Mathf.Lerp(baseRadius, topRadius, proj.heightProgress);
            
            // Add spiral rotation based on height
            float spiralAngle = proj.startAngle + (proj.heightProgress * spiralTightness * 360f);
            float currentRotation = spiralAngle + (rotationSpeed * Time.time * proj.speedMultiplier);
            
            float angleRad = currentRotation * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(angleRad) * currentRadius,
                currentHeight,
                Mathf.Sin(angleRad) * currentRadius
            ) + proj.randomOffset * (1f - proj.heightProgress * 0.5f); // Reduce noise at top

            proj.obj.transform.position = transform.position + offset;

            // Orient to face tangent (direction of movement)
            Vector3 tangent = new Vector3(-Mathf.Sin(angleRad), 0.2f, Mathf.Cos(angleRad));
            proj.obj.transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
        }
    }

    IEnumerator StartMovementAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeMove);

        if (OpenWorldEnv.Current == null)
            yield break;

        Vector3 targetPos = OpenWorldEnv.Current.PlayerPos;
        
        // Flatten to XZ plane
        Vector3 directionFlat = targetPos - transform.position;
        directionFlat.y = 0f;
        _lockedTargetDir = directionFlat.normalized;
        
        _isMoving = true;
    }
}