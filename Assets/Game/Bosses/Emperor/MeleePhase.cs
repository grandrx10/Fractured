using UnityEngine;
using Game.Bosses;
using Cards.Environments;
using System.Collections;

namespace Game.Bosses
{
    [CreateAssetMenu(menuName = "BossAttacks/Emperor/MeleePhaseAttack")]
    public class MeleePhaseAttack : BossAttack
    {
        [Header("Teleport")]
        public float teleportDistance = 20f;

        [Header("Timing")]
        public float preMeleeDelay = 5f;

        [Header("Movement")]
        public float trackingSpeed = 8f;
        public float lungeSpeed = 24f;
        public float lungeInterval = 4f;
        public float lungeDuration = 0.5f;

        private Coroutine attackCoroutine;

        public override void StartAttack(GameObject bossObj)
        {
            base.StartAttack(bossObj);

            Boss boss = bossObj.GetComponent<Boss>();
            if (boss == null)
            {
                Debug.LogError("MeleePhaseAttack: Boss component missing.");
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

            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null && OpenWorldEnv.Current != null)
            {
                npcCommands.SetLookingAt(player);
            }

            /* ───────── TELEPORT ───────── */
            Vector2 randXZ = Random.insideUnitCircle.normalized * teleportDistance;
            Vector3 teleportPos = OpenWorldEnv.Current.PlayerPos + new Vector3(randXZ.x, 0f, randXZ.y);
            teleportPos.y = bossTf.position.y; // keep the boss's current height
            bossTf.position = teleportPos;


            /* ───────── WAIT ───────── */
            yield return new WaitForSeconds(preMeleeDelay);

            /* ───────── MELEE PHASE ───────── */
            float lungeTimer = 0f;
            float currentLungeTime = 0f;
            bool isLunging = false;
            Vector3 lungeDirection = Vector3.zero;

            while (true)
            {
                lungeTimer += Time.deltaTime;

                Vector3 targetPos = player.position;
                targetPos.y = bossTf.position.y; // keep boss y-level

                // Check if it's time to start a new lunge
                if (!isLunging && lungeTimer >= lungeInterval)
                {
                    isLunging = true;
                    currentLungeTime = 0f;
                    lungeTimer = 0f;

                    // Lock direction for lunge (horizontal only)
                    lungeDirection = (targetPos - bossTf.position).normalized;
                    lungeDirection.y = 0f;

                    if (npcCommands != null)
                        npcCommands.SetLookingAt(null);
                }

                // Handle lunging
                if (isLunging)
                {
                    currentLungeTime += Time.deltaTime;

                    if (currentLungeTime >= lungeDuration)
                    {
                        isLunging = false;
                        currentLungeTime = 0f;

                        if (npcCommands != null)
                            npcCommands.SetLookingAt(player);
                    }
                    else
                    {
                        bossTf.position += lungeDirection * lungeSpeed * Time.deltaTime;
                    }
                }
                else
                {
                    // Normal tracking movement (horizontal only)
                    Vector3 moveDir = (targetPos - bossTf.position).normalized;
                    moveDir.y = 0f;
                    bossTf.position += moveDir * trackingSpeed * Time.deltaTime;
                }

                yield return null;
            }
        }
    }
}
