using System.Collections;
using UnityEngine;
using Characters; // For PlayerSingleton

namespace Game.Bosses.Cyra
{
    [CreateAssetMenu(menuName = "BossAttacks/Cyra/VortexAttack")]
    public class VortexAttack : BossAttack
    {
        [Header("Spawn Settings")]
        public GameObject objectToSpawn;    // Prefab to spawn
        public int numberOfPrefabs = 6;     // How many prefabs in the circle
        public float spawnInterval = 0.5f;  // Time between spawns
        public float rotationOffset = 3f;   // Degrees to shift each spawn
        public float burstDuration = 2f;    // Spawn duration before pause
        public float pauseDuration = 0.5f;  // Pause duration after burst

        private Coroutine spawnRoutine;
        private Coroutine facePlayerRoutine;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            if (objectToSpawn == null)
            {
                Debug.LogError("VortexAttack: objectToSpawn not assigned!");
                return;
            }

            // Start spawning prefabs
            spawnRoutine = boss.GetComponent<MonoBehaviour>().StartCoroutine(SpawnRoutine(boss.transform));

            // Start facing player
            if (PlayerSingleton.Instance != null)
            {
                facePlayerRoutine = boss.GetComponent<MonoBehaviour>().StartCoroutine(FacePlayerRoutine(boss.transform));
            }
        }

        private IEnumerator SpawnRoutine(Transform bossTransform)
        {
            float currentRotationOffset = 0f;
            float burstTimer = 0f;

            while (isActive)
            {
                // Spawn prefabs in a circle
                float angleStep = 360f / numberOfPrefabs;
                for (int i = 0; i < numberOfPrefabs; i++)
                {
                    float angle = i * angleStep + currentRotationOffset;
                    float rad = angle * Mathf.Deg2Rad;
                    Vector3 spawnDir = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));

                    if (objectToSpawn != null)
                    {
                        Vector3 spawnPos = bossTransform.position + spawnDir;

                        Object.Instantiate(
                            objectToSpawn,
                            spawnPos,
                            Quaternion.LookRotation(spawnDir),
                            null // Not parented
                        );
                    }
                }

                currentRotationOffset += rotationOffset;
                burstTimer += spawnInterval;

                // Pause if burst duration reached
                if (burstTimer >= burstDuration)
                {
                    yield return new WaitForSeconds(pauseDuration);
                    burstTimer = 0f;
                }
                else
                {
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }

        private IEnumerator FacePlayerRoutine(Transform bossTransform)
        {
            while (isActive && PlayerSingleton.Instance != null)
            {
                Vector3 direction = (PlayerSingleton.Instance.transform.position - bossTransform.position).normalized;
                direction.y = 0f; // only rotate on Y axis
                if (direction.sqrMagnitude > 0f)
                    bossTransform.forward = direction;

                yield return null;
            }
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);

            if (spawnRoutine != null && boss != null)
                boss.GetComponent<MonoBehaviour>().StopCoroutine(spawnRoutine);

            if (facePlayerRoutine != null && boss != null)
                boss.GetComponent<MonoBehaviour>().StopCoroutine(facePlayerRoutine);
        }

        public override void Tick(GameObject boss) { }
    }
}
