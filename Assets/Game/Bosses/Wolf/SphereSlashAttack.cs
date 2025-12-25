using System.Collections;
using Characters;
using UnityEngine;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Wolf
{
    [CreateAssetMenu(menuName = "BossAttacks/Wolf/SphereSlashAttack")]
    public class SphereSlashAttack : BossAttack
    {
        [Header("Spawn Settings")]
        public GameObject slashPrefab;
        public GameObject warningPrefab;

        [Header("Attack Settings")]
        public float attackInterval = 1f;
        public float warningDuration = 1f;
        public float spawnRadius = 5f;
        public float bossHeightOffset = 5f;
        public int warningRadius = 5;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            MonoBehaviour runner = boss.GetComponent<MonoBehaviour>();
            if (runner != null)
                runner.StartCoroutine(SpawnLoop(boss));
        }

        private IEnumerator SpawnLoop(GameObject boss)
        {
            MonoBehaviour runner = boss.GetComponent<MonoBehaviour>();

            while (isActive && PlayerSingleton.Instance != null)
            {
                // Fire-and-forget one slash sequence
                runner.StartCoroutine(SpawnSingleSlash(boss));

                // Only control spawn rate here
                yield return new WaitForSeconds(attackInterval);
            }
        }

        private IEnumerator SpawnSingleSlash(GameObject boss)
        {
            // Choose a random position around player
            Vector3 playerPos = PlayerSingleton.Instance.GetPositionBelow();
            Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
            Vector3 targetPos = playerPos + new Vector3(offset2D.x, 0f, offset2D.y);

            // Spawn warning
            if (warningPrefab != null)
            {
                GameObject warningObj = Instantiate(warningPrefab, targetPos, Quaternion.identity);
                Warning w = warningObj.GetComponent<Warning>();
                if (w != null)
                {
                    w.Initialize(
                        warningRadius,
                        attackInterval,
                        Warning.WarningType.Grounded,
                        warningDuration
                    );
                }

                yield return new WaitForSeconds(warningDuration);

                Destroy(warningObj);
            }

            // Teleport boss above the target
            boss.transform.position = targetPos + Vector3.up * bossHeightOffset;

            // Spawn slash
            if (slashPrefab != null)
                Instantiate(slashPrefab, targetPos, Quaternion.identity);
        }
    }
}
