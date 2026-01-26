using System.Collections;
using UnityEngine;
using Cards.Environments;
using Game.Bosses.Projectiles;

namespace Game.Bosses
{
    [CreateAssetMenu(menuName = "BossAttacks/Rock/StoneAttack")]
    public class StoneAttack : BossAttack
    {
        [Header("Attack Settings")]
        public float attackInterval = 1f;      // How often stones fall
        public float warningDuration = 1.5f;   // Time before stone falls
        public float spawnHeight = 20f;        // Height above warning
        public float offset = 10f;

        [Header("Prefabs")]
        public GameObject stonePrefab;
        public GameObject warningPrefab;
        public float warningRadius = 3f;

        private GameObject bossRef;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);
            bossRef = boss;

            // Start the main spawning coroutine
            boss.GetComponent<MonoBehaviour>().StartCoroutine(SpawnStones());
        }

        private IEnumerator SpawnStones()
        {
            while (isActive)
            {
                if (OpenWorldEnv.Current.PlayerTransform == null)
                    yield break;

                // Get player's XZ position
                Vector3 playerPos = OpenWorldEnv.Current.PlayerTransform.position;

                // Random offset in XZ plane
                Vector2 randomOffset = Random.insideUnitCircle * offset; // radius around player
                Vector3 targetPos = playerPos + new Vector3(randomOffset.x, 0f, randomOffset.y);

                // Spawn the warning circle
                if (warningPrefab != null)
                {
                    Warning w = Instantiate(warningPrefab, targetPos, Quaternion.identity)
                        .GetComponent<Warning>();
                    w.Initialize(warningRadius, attackInterval, Warning.WarningType.Grounded, warningDuration);
                }

                // Spawn the stone above the warning
                if (stonePrefab != null)
                {
                    Vector3 spawnPos = targetPos + Vector3.up * spawnHeight;
                    GameObject stone = Instantiate(stonePrefab, spawnPos, Quaternion.identity);

                    Rigidbody rb = stone.GetComponent<Rigidbody>();
                    if (rb == null)
                        rb = stone.AddComponent<Rigidbody>();

                    rb.isKinematic = true;    // freeze until warning ends
                    rb.useGravity = false;

                    // After warningDuration, let the stone fall
                    bossRef.GetComponent<MonoBehaviour>().StartCoroutine(DropStoneAfter(stone, rb, warningDuration));
                }

                yield return new WaitForSeconds(attackInterval);
            }
        }


        private IEnumerator DropStoneAfter(GameObject stone, Rigidbody rb, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (stone != null && rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }
}
