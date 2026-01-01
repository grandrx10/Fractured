using System.Collections;
using Cards.Environments;
using Characters;
using Game.Bosses.Projectiles;
using UnityEngine;

namespace Game.Bosses.Sylph
{
    [CreateAssetMenu(menuName = "BossAttacks/Sylph/WaterSplashAttack")]
    public class WaterSplashAttack : BossAttack
    {
        [Header("Spawn Settings")]
        public string spawnPointName;   // The boss point to fire from

        [Header("Attack Settings")]
        public float attackInterval = 0.5f;
        public float targetRadius = 5f;
        public float minPeakHeight = 2f;
        public float maxPeakHeight = 6f;
        public float splashDuration = 1f;

        [Header("Projectile Settings")]
        public GameObject projectilePrefab;

        [Header("Warning Settings")]
        public GameObject warningPrefab;
        public int warningRadius;
        public float warningDuration;

        private Transform spawnPoint;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            // Get spawn transform the same way RockToss does
            spawnPoint = boss.GetComponent<Boss>().GetPointTransform(spawnPointName);

            if (spawnPoint == null)
            {
                Debug.LogError("WaterSplashAttack: Spawn point not found: " + spawnPointName);
                return;
            }
            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null)
            {
                npcCommands.SetLookingAt(OpenWorldEnv.Current.PlayerTransform);
            }

            boss.GetComponent<MonoBehaviour>().StartCoroutine(ContinuousSplash(boss));
        }

        private IEnumerator ContinuousSplash(GameObject boss)
        {
            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            while (isActive)
            {
                // Ground position under the player
                Vector3 playerPos = OpenWorldEnv.Current.GetBossTargetGrounded();

                // Random splash target around player
                Vector2 randomOffset = Random.insideUnitCircle * targetRadius;
                Vector3 targetPos = playerPos + new Vector3(randomOffset.x, 0f, randomOffset.y);

                // Use the assigned spawn point instead of boss.position
                Vector3 spawnPos = spawnPoint.position;

                // Spawn warning ring
                if (warningPrefab != null)
                {
                    Warning w = Instantiate(warningPrefab, targetPos, Quaternion.identity)
                        .GetComponent<Warning>();
                    w.Initialize(warningRadius, attackInterval, Warning.WarningType.Grounded, warningDuration);
                }

                // Spawn splash projectile
                if (projectilePrefab != null)
                {
                    GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                    Rigidbody rb = proj.GetComponent<Rigidbody>();
                    if (rb == null)
                        rb = proj.AddComponent<Rigidbody>();

                    rb.isKinematic = true;
                    rb.useGravity = false;

                    boss.GetComponent<MonoBehaviour>().StartCoroutine(
                        LerpParabola(proj, rb, spawnPos, targetPos, splashDuration)
                    );
                }

                yield return new WaitForSeconds(attackInterval);
            }
            if (npcCommands != null)
            {
                npcCommands.SetLookingAt(null);
            }
        }

        private IEnumerator LerpParabola(GameObject projectile, Rigidbody rb,
            Vector3 startPos, Vector3 targetPos, float duration)
        {
            float distance = Vector3.Distance(startPos, targetPos);

            float normalizedDistance = Mathf.Clamp01(distance / targetRadius);
            float peakHeight = Mathf.Lerp(maxPeakHeight, minPeakHeight, normalizedDistance);

            float gravity = 2f * peakHeight / Mathf.Pow(duration / 2f, 2);
            float initialVerticalSpeed = gravity * (duration / 2f);

            float time = 0f;
            while (time < duration && projectile != null)
            {
                Vector3 horiz = Vector3.Lerp(startPos, targetPos, time / duration);
                float y = startPos.y + initialVerticalSpeed * time - 0.5f * gravity * time * time;

                Vector3 newPos = new Vector3(horiz.x, y, horiz.z);
                rb.MovePosition(newPos);

                time += Time.deltaTime;
                yield return null;
            }

            if (projectile != null)
                rb.MovePosition(targetPos);
        }
    }
}
