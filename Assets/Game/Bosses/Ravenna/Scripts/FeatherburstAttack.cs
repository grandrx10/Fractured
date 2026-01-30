using System.Collections;
using UnityEngine;
using Cards.Environments;
using Characters;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Ravenna
{
    [CreateAssetMenu(menuName = "BossAttacks/Ravenna/FeatherburstAttack")]
    public class FeatherburstAttack : BossAttack
    {
        [Header("Spawn Settings")]
        public string spawnPointName;
        public int featherCount = 12;

        [Header("Attack Timing")]
        public float attackInterval = 2f;

        [Header("Targeting")]
        public float targetRadius = 6f;

        [Header("Projectile Settings")]
        public GameObject projectilePrefab;
        public float moveSpeed = 12f;
        public float turnSpeed = 6f;
        public float killDistance = 0.75f;

        [Header("Feather Variation")]
        public float spawnStagger = 0.05f;
        public float speedVariance = 2f;
        public float turnSpeedVariance = 2f;

        [Header("Warning Settings")]
        public GameObject warningPrefab;
        public int warningRadius = 3;
        public float warningDuration = 1.5f;

        private Transform spawnPoint;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            spawnPoint = boss.GetComponent<Boss>().GetPointTransform(spawnPointName);
            if (spawnPoint == null)
            {
                Debug.LogError("FeatherburstAttack: Spawn point not found: " + spawnPointName);
                return;
            }

            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null && OpenWorldEnv.Current != null)
            {
                npcCommands.SetLookingAt(OpenWorldEnv.Current.PlayerTransform);
            }

            boss.GetComponent<MonoBehaviour>()
                .StartCoroutine(FeatherburstLoop(boss));
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);

            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null)
            {
                npcCommands.SetLookingAt(null);
            }
        }

        private IEnumerator FeatherburstLoop(GameObject boss)
        {
            while (isActive)
            {
                Vector3 playerGround = OpenWorldEnv.Current.GetBossTargetGrounded();
                Vector2 randomOffset = Random.insideUnitCircle * targetRadius;
                Vector3 targetPos = playerGround + new Vector3(randomOffset.x, 0f, randomOffset.y);

                if (warningPrefab != null)
                {
                    Warning w = Instantiate(
                        warningPrefab,
                        targetPos,
                        Quaternion.identity
                    ).GetComponent<Warning>();

                    w.Initialize(
                        warningRadius,
                        attackInterval,
                        Warning.WarningType.Grounded,
                        warningDuration
                    );
                }

                yield return boss.GetComponent<MonoBehaviour>()
                    .StartCoroutine(SpawnFeatherBurst(targetPos));

                yield return new WaitForSeconds(attackInterval);
            }

            SetTrigger(boss, "next");
        }

        private IEnumerator SpawnFeatherBurst(Vector3 targetPos)
        {
            for (int i = 0; i < featherCount; i++)
            {
                SpawnFeather(targetPos);
                yield return new WaitForSeconds(spawnStagger);
            }
        }

        private void SpawnFeather(Vector3 targetPos)
        {
            float tiltX = Random.Range(-15f, 15f);
            float tiltZ = Random.Range(-15f, 15f);

            Quaternion rot =
                spawnPoint.rotation *
                Quaternion.Euler(tiltX, tiltZ, 0f);

            GameObject proj = Instantiate(
                projectilePrefab,
                spawnPoint.position,
                rot
            );

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb == null)
                rb = proj.AddComponent<Rigidbody>();

            rb.useGravity = false;
            rb.linearDamping = 0f;

            FeatherHoming homing = proj.GetComponent<FeatherHoming>();
            if (homing == null)
                homing = proj.AddComponent<FeatherHoming>();

            float finalSpeed = moveSpeed + Random.Range(-speedVariance, speedVariance);
            float finalTurnSpeed = turnSpeed + Random.Range(-turnSpeedVariance, turnSpeedVariance);

            homing.Initialize(
                targetPos,
                Mathf.Max(1f, finalSpeed),
                Mathf.Max(0.1f, finalTurnSpeed),
                killDistance
            );
        }
    }
}
