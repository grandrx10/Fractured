using System.Collections;
using UnityEngine;
using Cards.Environments;

namespace Game.Bosses
{
    [CreateAssetMenu(menuName = "BossAttacks/Emperor/SwipePhaseAttack")]
    public class SwipePhaseAttack : BossAttack
    {
        [Header("Movement")]
        public float dashSpeed = 25f;

        [Header("Timing")]
        public float slashDelay = 0.5f;       // delay after dash before slashing
        public float recoveryDelay = 0.5f;    // delay after slash before next dash

        [Header("Rhythm Pattern")]
        public int dashesPerCycle = 3;        // number of dash-slash combos in "on" cycle
        public float offCycleDuration = 2f;   // pause duration during "off" cycle

        [Header("Summon")]
        public GameObject sweepPrefab;
        public string summonPointName;

        private Coroutine attackCoroutine;

        public override void StartAttack(GameObject bossObj)
        {
            base.StartAttack(bossObj);

            Boss boss = bossObj.GetComponent<Boss>();
            if (boss == null)
            {
                Debug.LogError("SwipePhaseAttack: Boss component missing.");
                return;
            }

            attackCoroutine = boss.StartCoroutine(AttackRoutine(boss));
        }

        public override void EndAttack(GameObject bossObj)
        {
            base.EndAttack(bossObj);

            if (attackCoroutine != null)
            {
                Boss boss = bossObj.GetComponent<Boss>();
                if (boss != null)
                {
                    boss.StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }
            }
        }

        private IEnumerator AttackRoutine(Boss boss)
        {
            Transform player = OpenWorldEnv.Current.PlayerTransform;
            Transform bossTf = boss.transform;
            NpcCommands npc = boss.GetComponent<NpcCommands>();

            while (true)
            {
                // ON CYCLE: Perform multiple dash-slash combos
                for (int i = 0; i < dashesPerCycle; i++)
                {
                    // Lock onto player position
                    Vector3 toPlayer = player.position - bossTf.position;
                    Vector3 dashDirection = new Vector3(toPlayer.x, 0f, toPlayer.z).normalized;
                    float dashDistance = new Vector3(toPlayer.x, 0f, toPlayer.z).magnitude;

                    // Dash to player while tracking
                    float dashDuration = dashDistance / dashSpeed;
                    float dashTimer = 0f;

                    while (dashTimer < dashDuration)
                    {
                        // Keep facing player during dash
                        if (npc != null)
                            npc.SetLookingAt(player);

                        bossTf.position += dashDirection * dashSpeed * Time.deltaTime;
                        dashTimer += Time.deltaTime;
                        yield return null;
                    }

                    // Lock direction - stop turning to player
                    if (npc != null)
                        npc.SetLookingAt(null);

                    // Wait before slashing (boss is now locked in place)
                    yield return new WaitForSeconds(slashDelay);

                    // Spawn slash
                    Transform summonPoint = boss.GetPointTransform(summonPointName);
                    if (summonPoint != null && sweepPrefab != null)
                    {
                        Instantiate(sweepPrefab, summonPoint.position, summonPoint.rotation);
                    }

                    // Recovery after slash
                    yield return new WaitForSeconds(recoveryDelay);
                }

                // OFF CYCLE: Pause
                yield return new WaitForSeconds(offCycleDuration);
            }
        }
    }
}