using System.Collections;
using Cards.Environments;
using UnityEngine;
using Characters;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Dorian
{
    [CreateAssetMenu(menuName = "BossAttacks/Dorian/NovemberRainAttack")]
    public class NovemberRainAttack : BossAttack
    {
        [Header("Attack Prefabs")]
        public GameObject projectilePrefab;
        public GameObject warningPrefab;

        [Header("Attack Settings")]
        public float targetRadius = 30f;     // Radius around player
        public float spawnHeight = 50f;      // Height above target
        public float projectileSpeed = 20f;  // Speed to hit the ground
        public float spawnInterval = 2f;     // Time between spawns
        public float warningDelay = 0.5f;    // Delay before projectile spawns after warning

        [Header("Warning Settings")]
        public float warningDuration = 1.5f; // Warning visibility duration
        public float warningRadius = 2f;

        [Header("Rain Angle Settings")]
        public float fallAngle = 30f;        // Angle above horizontal
        public Vector3 fallDirection = Vector3.forward; // Direction in XZ plane

        private Coroutine spawnCoroutine;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);
            MonoBehaviour mb = boss.GetComponent<MonoBehaviour>();
            if (mb != null)
                spawnCoroutine = mb.StartCoroutine(SpawnLoop(mb));
        }

        private IEnumerator SpawnLoop(MonoBehaviour context)
        {
            while (isActive)
            {
                Vector3 playerPos = OpenWorldEnv.Current.GetBossTargetGrounded();

                // Random horizontal offset around player
                Vector2 offset = Random.insideUnitCircle * targetRadius;
                Vector3 targetGroundPos = playerPos + new Vector3(offset.x, 0f, offset.y);

                // Spawn warning immediately
                if (warningPrefab != null)
                {
                    GameObject warningObj = Object.Instantiate(
                        warningPrefab,
                        targetGroundPos,
                        Quaternion.identity
                    );
                    Warning w = warningObj.GetComponent<Warning>();
                    if (w != null)
                        w.Initialize((int)warningRadius, warningDuration, Warning.WarningType.Grounded, warningDuration);
                }

                // Spawn projectile after warningDelay
                if (projectilePrefab != null)
                {
                    // Compute spawn position based on fixed global fall angle
                    Vector3 horizontalDir = new Vector3(fallDirection.x, 0f, fallDirection.z).normalized;
                    Vector3 horizontalOffset = horizontalDir * spawnHeight / Mathf.Tan(fallAngle * Mathf.Deg2Rad);
                    Vector3 spawnPos = targetGroundPos - horizontalOffset + Vector3.up * spawnHeight;

                    GameObject proj = Object.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                    // Rotate projectile to point along the fixed fall angle
                    Vector3 fallVector = (targetGroundPos - spawnPos).normalized;
                    proj.transform.rotation = Quaternion.LookRotation(fallVector);

                    context.StartCoroutine(FallProjectile(proj, targetGroundPos, projectileSpeed));
                }

                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private IEnumerator FallProjectile(GameObject proj, Vector3 target, float speed)
        {
            if (proj == null)
                yield break;

            Vector3 startPos = proj.transform.position;
            float distance = Vector3.Distance(startPos, target);
            float duration = distance / speed;
            float elapsed = 0f;

            while (elapsed < duration && proj != null)
            {
                elapsed += Time.deltaTime;
                proj.transform.position = Vector3.Lerp(startPos, target, elapsed / duration);
                yield return null;
            }

            if (proj != null)
                proj.transform.position = target;
        }

        public override void EndAttack(GameObject boss)
        {
            if (spawnCoroutine != null && boss != null)
            {
                MonoBehaviour mb = boss.GetComponent<MonoBehaviour>();
                if (mb != null)
                    mb.StopCoroutine(spawnCoroutine);
            }
            base.EndAttack(boss);
        }
    }
}
