using System.Collections;
using Cards.Environments;
using Characters;
using Game.Bosses.Projectiles;
using UnityEngine;

namespace Game.Bosses.Sylph
{
    [CreateAssetMenu(menuName = "BossAttacks/Sylph/WaterFountainAttack")]
    public class WaterFountainAttack : BossAttack
    {
        [Header("Fountain Settings")]
        public GameObject fountainPrefab;
        public float spawnRadius = 6f;

        [Header("Warning Settings")]
        public GameObject warningPrefab;
        public float warningDelay = 1.0f;
        public int warningRadius = 2;
        public float warningDuration = 1.0f;

        [Header("Timing")]
        public float interval = 0.5f;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null)
            {
                npcCommands.SetLookingAt(OpenWorldEnv.Current.PlayerTransform);
            }

            var runner = boss.GetComponent<RoutineRunner>();
            if (runner != null)
            {
                runner.RunRoutine(FountainRoutine(boss, runner, npcCommands));
            }
            else
            {
                Debug.LogWarning("Boss missing RoutineRunner component!");
            }
        }

        private IEnumerator FountainRoutine(GameObject boss, RoutineRunner runner, NpcCommands npcCommands)
        {
            while (isActive)
            {
                Vector3 basePos = OpenWorldEnv.Current.GetBossTargetGrounded();

                // Random point around player
                Vector2 offset = Random.insideUnitCircle * spawnRadius;
                Vector3 targetPos = basePos + new Vector3(offset.x, 0f, offset.y);

                GameObject warningGO = null;

                // Spawn warning
                if (warningPrefab != null)
                {
                    warningGO = Instantiate(warningPrefab, targetPos, Quaternion.identity);
                    Warning w = warningGO.GetComponent<Warning>();
                    if (w != null)
                        w.Initialize(warningRadius, warningDelay, Warning.WarningType.Grounded, warningDuration);
                }

                // Spawn fountain after warningDelay asynchronously
                if (fountainPrefab != null)
                    runner.RunRoutine(SpawnFountainDelayed(targetPos, warningDelay, warningGO));

                // Wait only for the interval between fountains
                yield return new WaitForSeconds(interval);
            }

            // Stop looking at player when attack ends
            if (npcCommands != null)
                npcCommands.SetLookingAt(null);
        }

        private IEnumerator SpawnFountainDelayed(Vector3 targetPos, float delay, GameObject warningGO)
        {
            yield return new WaitForSeconds(delay);

            // Destroy warning when fountain rises
            if (warningGO != null)
                Destroy(warningGO);

            // Spawn fountain
            GameObject fountain = Instantiate(fountainPrefab, targetPos, Quaternion.identity);

            // Align Top child to targetPos
            Transform top = fountain.transform.Find("Top");
            if (top != null)
            {
                Vector3 worldOffset = top.position - fountain.transform.position;
                fountain.transform.position = targetPos - worldOffset;
            }
            else
            {
                Debug.LogWarning("WaterFountain prefab missing child named 'Top'");
            }
        }
    }
}
