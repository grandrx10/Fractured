using System.Collections;
using UnityEngine;
using Game.Bosses;
using Characters;

namespace Game.Bosses.Cyra
{
    [CreateAssetMenu(menuName = "BossAttacks/Cyra/SunRingAttackHard")]
    public class SunRingAttackHard : BossAttack
    {
        [Header("SunRing Settings")]
        public string spawnPointName;
        public GameObject sunRingPrefab;
        public float sunRingSpawnInterval = 1f;

        [Header("SoulBall Settings")]
        public GameObject soulBallPrefab;
        public float soulBallSpawnInterval = 3f;

        private Transform spawnPoint;
        private Coroutine sunRingRoutine;
        private Coroutine soulBallRoutine;
        private Coroutine lookAtPlayerRoutine;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            Boss bossComp = boss.GetComponent<Boss>();
            if (bossComp == null)
            {
                Debug.LogError("SunRingAttackHard: Boss component missing!");
                return;
            }

            spawnPoint = bossComp.GetPointTransform(spawnPointName);
            if (spawnPoint == null)
            {
                Debug.LogError("SunRingAttackHard: Spawn point not found: " + spawnPointName);
                return;
            }

            MonoBehaviour runner = boss.GetComponent<MonoBehaviour>();

            sunRingRoutine = runner.StartCoroutine(SunRingSpawnRoutine());
            soulBallRoutine = runner.StartCoroutine(SoulBallSpawnRoutine(boss.transform));

            if (PlayerSingleton.Instance != null)
            {
                lookAtPlayerRoutine = runner.StartCoroutine(
                    LookAtPlayerCoroutine(boss.transform)
                );
            }
        }

        private IEnumerator SunRingSpawnRoutine()
        {
            while (isActive)
            {
                if (sunRingPrefab != null && spawnPoint != null)
                {
                    Instantiate(
                        sunRingPrefab,
                        spawnPoint.position,
                        spawnPoint.rotation,
                        spawnPoint
                    );
                }

                yield return new WaitForSeconds(sunRingSpawnInterval);
            }
        }

        private IEnumerator SoulBallSpawnRoutine(Transform bossTransform)
        {
            while (isActive)
            {
                yield return new WaitForSeconds(soulBallSpawnInterval);

                if (!isActive || soulBallPrefab == null || spawnPoint == null)
                    continue;

                GameObject soulBall = Instantiate(
                    soulBallPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

                SoulBall sb = soulBall.GetComponent<SoulBall>();
                if (sb != null)
                {
                    sb.SetBoss(bossTransform);
                }
            }
        }

        private IEnumerator LookAtPlayerCoroutine(Transform bossTransform)
        {
            while (isActive && PlayerSingleton.Instance != null)
            {
                Vector3 dir =
                    PlayerSingleton.Instance.transform.position - bossTransform.position;
                dir.y = 0f;

                if (dir.sqrMagnitude > 0.001f)
                    bossTransform.forward = dir.normalized;

                yield return null;
            }
        }

        public override void EndAttack(GameObject boss)
        {
            MonoBehaviour runner = boss.GetComponent<MonoBehaviour>();

            if (runner != null)
            {
                if (sunRingRoutine != null)
                    runner.StopCoroutine(sunRingRoutine);

                if (soulBallRoutine != null)
                    runner.StopCoroutine(soulBallRoutine);

                if (lookAtPlayerRoutine != null)
                    runner.StopCoroutine(lookAtPlayerRoutine);
            }

            base.EndAttack(boss);
        }

        public override void Tick(GameObject boss) { }
    }
}
