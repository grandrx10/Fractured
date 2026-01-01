using System.Collections;
using Cards.Environments;
using UnityEngine;
using Game.Bosses;
using Characters; // For PlayerSingleton

namespace Game.Bosses.Cyra
{
    [CreateAssetMenu(menuName = "BossAttacks/Cyra/SunRingAttack")]
    public class SunRingAttack : BossAttack
    {
        [Header("Spawn Settings")]
        public string spawnPointName;       // Named boss point to spawn from
        public GameObject objectToSpawn;    // SunRing prefab
        public float spawnInterval = 1f;    // Time between spawns

        [Header("SoulBall Settings")]
        public GameObject soulBallPrefab;   // SoulBall prefab to spawn at end

        private Transform spawnPoint;
        private Coroutine spawnRoutine;
        private Coroutine lookAtPlayerRoutine;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            if (objectToSpawn == null)
            {
                Debug.LogError("SunRingAttack: objectToSpawn not assigned!");
                return;
            }

            // Get spawn transform from boss
            spawnPoint = boss.GetComponent<Boss>()?.GetPointTransform(spawnPointName);
            if (spawnPoint == null)
            {
                Debug.LogError("SunRingAttack: Spawn point not found: " + spawnPointName);
                return;
            }

            // Start spawning SunRings continuously
            spawnRoutine = boss.GetComponent<MonoBehaviour>().StartCoroutine(SpawnRoutine());

            // Start rotating boss to face player continuously
            
            lookAtPlayerRoutine = boss.GetComponent<MonoBehaviour>()
                .StartCoroutine(LookAtPlayerCoroutine(boss.transform));
            
        }

        private IEnumerator SpawnRoutine()
        {
            while (isActive)
            {
                if (objectToSpawn != null && spawnPoint != null)
                {
                    Instantiate(
                        objectToSpawn,
                        spawnPoint.position,
                        spawnPoint.rotation,
                        spawnPoint // parent initially
                    );
                }

                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private IEnumerator LookAtPlayerCoroutine(Transform bossTransform)
        {
            while (isActive)
            {
                Vector3 playerPos = OpenWorldEnv.Current.PlayerPos;
                Vector3 direction = (playerPos - bossTransform.position).normalized;
                direction.y = 0f; // keep rotation on Y axis only

                if (direction.sqrMagnitude > 0f)
                    bossTransform.forward = direction;

                yield return null;
            }
        }

        public override void EndAttack(GameObject boss)
        {

            // Stop the spawning coroutine
            if (spawnRoutine != null && boss != null)
            {
                boss.GetComponent<MonoBehaviour>().StopCoroutine(spawnRoutine);
            }

            // Stop the LookAtPlayer coroutine
            if (lookAtPlayerRoutine != null && boss != null)
            {
                boss.GetComponent<MonoBehaviour>().StopCoroutine(lookAtPlayerRoutine);
            }

            // Spawn SoulBall at the spawn point and pass boss reference
            if (soulBallPrefab != null && spawnPoint != null && boss != null)
            {
                GameObject soulBall = Instantiate(
                    soulBallPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation,
                    spawnPoint // optionally parent initially
                );

                SoulBall sb = soulBall.GetComponent<SoulBall>();
                if (sb != null)
                {
                    sb.SetBoss(boss.transform);
                }
            }
            base.EndAttack(boss);
        }

        public override void Tick(GameObject boss) { }
    }
}
