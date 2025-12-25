using System.Collections;
using UnityEngine;
using Characters;

namespace Game.Bosses
{
    [CreateAssetMenu(menuName = "BossAttacks/Wolf/ScissorCircleAttack")]
    public class ScissorCircleAttack : BossAttack
    {
        [Header("Scissor Settings")]
        public GameObject scissorPrefab;
        public int scissorCount = 12;

        [Header("Orbit Settings")]
        public float orbitRadius = 5f;
        public float orbitSpeed = 90f; // degrees per second
        public float orbitDuration = 2f;

        [Header("Launch Settings")]
        public float launchInterval = 0.15f;
        public float launchSpeed = 20f;

        [Header("Boss Movement")]
        public float moveTowardPlayerSpeed = 4f;
        public float stopDistance = 2f;

        private MonoBehaviour coroutineRunner;
        private Transform bossTransform;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            coroutineRunner = boss.GetComponent<MonoBehaviour>();
            bossTransform = boss.transform;

            if (coroutineRunner == null)
            {
                Debug.LogError("ScissorCircleAttack: Boss has no MonoBehaviour");
                return;
            }

            coroutineRunner.StartCoroutine(AttackLoop());
        }

        private IEnumerator AttackLoop()
        {
            // Run boss movement in parallel
            coroutineRunner.StartCoroutine(MoveTowardPlayer());

            while (isActive && PlayerSingleton.Instance != null)
            {
                yield return ExecuteOneCircle();
            }
        }

        private IEnumerator ExecuteOneCircle()
        {
            GameObject[] scissors = new GameObject[scissorCount];
            ScissorProjectile[] projectiles = new ScissorProjectile[scissorCount];

            // Spawn circle
            for (int i = 0; i < scissorCount; i++)
            {
                float angle = (360f / scissorCount) * i;
                Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * orbitRadius;
                Vector3 spawnPos = bossTransform.position + offset;

                Quaternion rotation = Quaternion.LookRotation(offset.normalized);

                scissors[i] = Instantiate(scissorPrefab, spawnPos, rotation);
                projectiles[i] = scissors[i].GetComponent<ScissorProjectile>();

                if (projectiles[i] != null)
                {
                    projectiles[i].Initialize(
                        bossTransform,
                        angle,
                        orbitRadius,
                        orbitSpeed
                    );
                }
            }

            // Orbit phase
            yield return new WaitForSeconds(orbitDuration);

            // Launch phase
            for (int i = 0; i < scissorCount; i++)
            {
                if (scissors[i] != null && projectiles[i] != null)
                {
                    projectiles[i].Launch(launchSpeed);
                }

                yield return new WaitForSeconds(launchInterval);
            }
        }

        private IEnumerator MoveTowardPlayer()
        {
            while (isActive && PlayerSingleton.Instance != null)
            {
                Vector3 playerPos = PlayerSingleton.Instance.transform.position;
                Vector3 toPlayer = playerPos - bossTransform.position;
                toPlayer.y = 0f;

                // Rotate to face player
                if (toPlayer != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(toPlayer.normalized);
                    bossTransform.rotation = Quaternion.RotateTowards(
                        bossTransform.rotation,
                        targetRotation,
                        360f * Time.deltaTime // rotation speed in degrees per second
                    );
                }

                // Move toward player if farther than stopDistance
                float distance = toPlayer.magnitude;
                if (distance > stopDistance)
                {
                    Vector3 moveDir = toPlayer.normalized;
                    bossTransform.position += moveDir * moveTowardPlayerSpeed * Time.deltaTime;
                }

                yield return null;
            }
        }

    }
}
