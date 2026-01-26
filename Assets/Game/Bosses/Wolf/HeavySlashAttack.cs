using System.Collections;
using Cards.Environments;
using Characters;
using UnityEngine;

namespace Game.Bosses.Wolf
{
    [CreateAssetMenu(menuName = "BossAttacks/Wolf/HeavySlashAttack")]
    public class HeavySlashAttack : BossAttack
    {
        [Header("Spawn Settings")]
        public string spawnPointName;
        public GameObject slashPrefab;

        [Header("Attack Settings")]
        public float attackInterval = 0.5f; // Time between slashes

        [Header("Rotation Settings")]
        public float minRotationDegrees = 30f;
        public float maxRotationDegrees = 90f;
        public float rotationSpeed = 180f;

        [Header("Movement Settings (Slash)")]
        public float moveSpeed = 20f;
        public float travelDistance = 10f;

        [Header("Pre-Attack Movement (Boss)")]
        public string movePointName;
        public float moveToPointSpeed = 5f;
        public float arriveThreshold = 0.1f;

        [Header("Random Spawn Offset")]
        public Vector3 minOffset = Vector3.zero;
        public Vector3 maxOffset = Vector3.zero;

        private Transform spawnPoint;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            Boss bossComp = boss.GetComponent<Boss>();
            if (bossComp == null)
            {
                Debug.LogError("HeavySlashAttack: Boss component missing");
                return;
            }

            spawnPoint = bossComp.GetPointTransform(spawnPointName);
            if (spawnPoint == null)
            {
                Debug.LogError($"HeavySlashAttack: Spawn point not found: {spawnPointName}");
                return;
            }

            Transform movePoint = bossComp.GetPointTransform(movePointName);
            if (movePoint == null)
            {
                Debug.LogError($"HeavySlashAttack: Move point not found: {movePointName}");
                return;
            }

            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null)
                npcCommands.SetLookingAt(OpenWorldEnv.Current.PlayerTransform);

            // Make boss face forward (its own local forward)
            boss.transform.forward = movePoint.forward;

            MonoBehaviour runner = boss.GetComponent<MonoBehaviour>();
            if (runner != null)
                runner.StartCoroutine(MoveThenAttack(boss.transform, movePoint, runner));
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);

            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null)
                npcCommands.SetLookingAt(null);
        }

        private IEnumerator MoveThenAttack(Transform bossTransform, Transform target, MonoBehaviour runner)
        {
            while (Vector3.Distance(bossTransform.position, target.position) > arriveThreshold)
            {
                bossTransform.position = Vector3.MoveTowards(
                    bossTransform.position,
                    target.position,
                    moveToPointSpeed * Time.deltaTime
                );

                yield return null;
            }

            bossTransform.position = target.position;

            // Start slash loop continuously
            yield return runner.StartCoroutine(FireLoop(runner));
        }

        private IEnumerator FireLoop(MonoBehaviour runner)
        {
            while (isActive)
            {
                // Calculate random offset
                Vector3 offset = new Vector3(
                    Random.Range(minOffset.x, maxOffset.x),
                    Random.Range(minOffset.y, maxOffset.y),
                    Random.Range(minOffset.z, maxOffset.z)
                );

                Vector3 spawnPos = spawnPoint.position + offset;

                GameObject slash = Instantiate(slashPrefab, spawnPos, spawnPoint.rotation);

                // Fire-and-forget: slashes move independently
                runner.StartCoroutine(RotateThenMove(slash));

                yield return new WaitForSeconds(attackInterval);
            }
        }

        private IEnumerator RotateThenMove(GameObject slash)
        {
            if (slash == null)
                yield break;

            float targetRotation = Random.Range(minRotationDegrees, maxRotationDegrees);
            float rotated = 0f;

            // Rotate in place
            while (rotated < targetRotation)
            {
                float step = rotationSpeed * Time.deltaTime;
                float remaining = targetRotation - rotated;
                step = Mathf.Min(step, remaining);

                slash.transform.Rotate(Vector3.forward, step, Space.Self);
                rotated += step;

                yield return null;
            }

            // Move forward
            Vector3 direction = slash.transform.forward;
            float traveled = 0f;

            while (traveled < travelDistance && slash != null)
            {
                float step = moveSpeed * Time.deltaTime;
                slash.transform.position += direction * step;
                traveled += step;

                yield return null;
            }

            if (slash != null)
                Destroy(slash);
        }
    }
}
