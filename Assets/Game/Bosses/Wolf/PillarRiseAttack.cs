using System.Collections;
using Characters;
using UnityEngine;
using Game.Bosses;

namespace Game.Bosses.Wolf
{
    [CreateAssetMenu(menuName = "BossAttacks/Wolf/PillarRiseAttack")]
    public class PillarRiseAttack : BossAttack
    {
        [Header("Pillar Settings")]
        public GameObject pillarPrefab;

        [Header("Spawn Settings")]
        public float spawnInterval = 0.6f;
        public float spawnRadius = 70f;

        [Header("Rise Settings")]
        public float riseSpeed = 15f;
        public float riseHeight = 12f;

        [Header("Rotation Settings")]
        [Tooltip("Maximum tilt angle from vertical (0 = straight up, 90 = horizontal)")]
        public float maxTiltAngle = 30f;

        [Header("Floor Settings")]
        public string floorPointName; // Named point for the floor

        private MonoBehaviour coroutineRunner;
        private Transform floorTransform;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            coroutineRunner = boss.GetComponent<MonoBehaviour>();
            if (coroutineRunner == null)
            {
                Debug.LogError("Boss doesn't have a MonoBehaviour component!");
                return;
            }

            // Get floor transform from boss
            Boss bossComp = boss.GetComponent<Boss>();
            if (bossComp == null)
            {
                Debug.LogError("Boss component missing!");
                return;
            }

            floorTransform = bossComp.GetPointTransform(floorPointName);
            if (floorTransform == null)
            {
                Debug.LogError($"Floor point '{floorPointName}' not found on boss!");
                return;
            }

            // Start spawning pillars
            coroutineRunner.StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (isActive && PlayerSingleton.Instance != null)
            {
                SpawnPillar();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnPillar()
        {
            if (coroutineRunner == null || PlayerSingleton.Instance == null || floorTransform == null)
                return;

            Transform player = PlayerSingleton.Instance.transform;

            // Random point in radius around player (XZ plane)
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = new Vector3(
                player.position.x + randomCircle.x,
                floorTransform.position.y, // Use floor Y
                player.position.z + randomCircle.y
            );

            // Random rotation
            float randomYRotation = Random.Range(0f, 360f);
            float tiltX = Random.Range(-maxTiltAngle, maxTiltAngle);
            float tiltZ = Random.Range(-maxTiltAngle, maxTiltAngle);
            Quaternion rotation = Quaternion.Euler(tiltX, randomYRotation, tiltZ);

            // Spawn pillar
            GameObject pillar = Instantiate(pillarPrefab, spawnPos, rotation);

            // Disable collider during rise
            Collider pillarCollider = pillar.GetComponent<Collider>();
            if (pillarCollider != null) pillarCollider.enabled = false;

            // Start rise coroutine
            coroutineRunner.StartCoroutine(RisePillar(pillar.transform, pillarCollider));
        }

        private IEnumerator RisePillar(Transform pillar, Collider pillarCollider)
        {
            Vector3 startPos = pillar.position;
            Vector3 riseDirection = pillar.up;
            Vector3 targetPos = startPos + riseDirection * riseHeight;

            float distanceMoved = 0f;
            while (pillar != null && distanceMoved < riseHeight)
            {
                float moveAmount = riseSpeed * Time.deltaTime;
                pillar.position += riseDirection * moveAmount;
                distanceMoved += moveAmount;
                yield return null;
            }

            if (pillar != null)
            {
                pillar.position = targetPos;
                if (pillarCollider != null) pillarCollider.enabled = true;
            }
        }
    }
}
