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

        [Header("Summon")]
        public GameObject summonPrefab;
        public string summonPointName;

        [Header("Timing")]
        public float preMeleeDelay = 5f;

        [Header("Movement")]
        public float trackingSpeed = 8f;
        public float lungeSpeed = 24f;
        public float lungeInterval = 4f;
        public float lungeDuration = 0.5f;

        [Header("End")]
        public string endPointName;
        public float endLerpDuration = 1.5f;

        private Coroutine attackCoroutine;
        private Coroutine endLerpCoroutine;

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
            
            Boss boss = bossObj.GetComponent<Boss>();
            if (boss != null)
            {
                if (attackCoroutine != null)
                {
                    boss.StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }
                
                endLerpCoroutine = boss.StartCoroutine(EndLerpRoutine(boss));
            }
        }

        private IEnumerator AttackRoutine(Boss boss)
        {
            Transform player = OpenWorldEnv.Current.PlayerTransform;
            Transform bossTf = boss.transform;

            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null && OpenWorldEnv.Current != null)
            {
                npcCommands.SetLookingAt(OpenWorldEnv.Current.PlayerTransform);
            }

            /* ───────── TELEPORT ───────── */
            Vector2 randXZ = Random.insideUnitCircle.normalized * teleportDistance;
            Vector3 teleportPos = player.position + new Vector3(randXZ.x, 0f, randXZ.y);
            teleportPos.y = player.position.y;

            bossTf.position = teleportPos;
            teleportPos.y = player.position.y;

            bossTf.position = teleportPos;

            /* ───────── SUMMON ───────── */
            Transform summonPoint = boss.GetPointTransform(summonPointName);
            if (summonPoint != null && summonPrefab != null)
            {
                GameObject summon = Instantiate(
                    summonPrefab,
                    summonPoint.position,
                    summonPoint.rotation,
                    summonPoint
                );
            }

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
                bossTf.position = new Vector3(bossTf.position.x, player.position.y, bossTf.position.z);

                // Check if it's time to start a new lunge
                if (!isLunging && lungeTimer >= lungeInterval)
                {
                    isLunging = true;
                    currentLungeTime = 0f;
                    lungeTimer = 0f;
                    // Lock direction for lunge
                    lungeDirection = (targetPos - bossTf.position).normalized;
                    
                    // Stop looking at player during lunge
                    if (npcCommands != null)
                    {
                        npcCommands.SetLookingAt(null);
                    }
                }

                // Handle lunging
                if (isLunging)
                {
                    currentLungeTime += Time.deltaTime;
                    
                    if (currentLungeTime >= lungeDuration)
                    {
                        // End lunge, return to tracking
                        isLunging = false;
                        currentLungeTime = 0f;
                        
                        // Resume looking at player after lunge
                        if (npcCommands != null)
                        {
                            npcCommands.SetLookingAt(player);
                        }
                    }
                    else
                    {
                        // Move in locked lunge direction
                        bossTf.position += lungeDirection * lungeSpeed * Time.deltaTime;
                    }
                }
                else
                {
                    // Normal tracking movement
                    Vector3 moveDir = (targetPos - bossTf.position).normalized;
                    bossTf.position += moveDir * trackingSpeed * Time.deltaTime;
                }

                yield return null;
            }
        }

        private IEnumerator EndLerpRoutine(Boss boss)
        {
            Transform bossTf = boss.transform;
            Transform endPoint = boss.GetPointTransform(endPointName);
            
            if (endPoint != null)
            {
                Vector3 startPos = bossTf.position;
                Vector3 endPos = endPoint.position;

                float t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime / endLerpDuration;
                    bossTf.position = Vector3.Lerp(startPos, endPos, t);
                    yield return null;
                }

                bossTf.position = endPos;
            }

            endLerpCoroutine = null;
        }
    }
}