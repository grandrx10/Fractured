using System.Collections;
using Cards.Environments;
using Characters;
using Game.Bosses;
using Game.Bosses.Projectiles;
using UnityEngine;

namespace Game.Bosses.Rock
{
    [CreateAssetMenu(menuName = "BossAttacks/Rock/RockTossV2")]
    public class RockTossV2 : BossAttack
    {
        [Header("Named Points")]
        public string headPointName = "Head";
        public string headOutPointName = "HeadOutRockToss";
        public string rockSpawnPointName = "LaserOut";

        [Header("Rock Settings")]
        public GameObject rockPrefab;
        public float targetRadius = 5f;
        public float minPeakHeight = 2f;
        public float maxPeakHeight = 6f;
        public float tossDuration = 1f;

        [Header("Attack Timing")]
        public float attackInterval = 0.5f;

        [Header("Head Movement")]
        public float extendTime = 0.5f; // how long it takes for head to move out

        [Header("Warning Settings")]
        public GameObject warningPrefab;
        public int warningRadius = 2;
        public float warningDuration = 0.5f;

        private Transform bossTransform;

        private Transform headTransform;
        private Transform headOutTransform;
        private Transform spawnTransform;

        private Vector3 headStartLocalPos;
        private Quaternion headStartLocalRot;
        private Vector3 headStartWorldPos;

        private float elapsed;
        private bool headFullyExtended;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            bossTransform = boss.transform;

            Boss bossComp = boss.GetComponent<Boss>();

            headTransform = bossComp.GetPointTransform(headPointName);
            headOutTransform = bossComp.GetPointTransform(headOutPointName);
            spawnTransform = bossComp.GetPointTransform(rockSpawnPointName);

            if (headTransform != null)
            {
                headStartLocalPos = headTransform.localPosition;
                headStartLocalRot = headTransform.localRotation;
                headStartWorldPos = headTransform.position;
            }

            elapsed = 0f;
            headFullyExtended = false;

            // Start the tossing coroutine
            boss.GetComponent<MonoBehaviour>().StartCoroutine(RockTossRoutine(boss));
        }

        public override void Tick(GameObject boss)
        {
            if (headTransform == null || headOutTransform == null)
                return;

            elapsed += Time.deltaTime;

            // Smoothly move head toward HeadOutRockToss (POSITION AND ROTATION)
            float t = Mathf.Clamp01(elapsed / extendTime);
            headTransform.position = Vector3.Lerp(
                headStartWorldPos,
                headOutTransform.position,
                t
            );
            
            // Also lerp the rotation
            headTransform.rotation = Quaternion.Lerp(
                headTransform.rotation,
                headOutTransform.rotation,
                t
            );

            // Mark as fully extended when complete
            if (t >= 1f && !headFullyExtended)
            {
                headFullyExtended = true;
            }
        }

        private IEnumerator RockTossRoutine(GameObject boss)
        {
            // Wait for head to be fully extended
            while (!headFullyExtended)
            {
                yield return null;
            }

            while (isActive)
            {
                // Pick random target around player
                Vector3 playerPos = OpenWorldEnv.Current.GetBossTargetGrounded();
                Vector2 offset = Random.insideUnitCircle * targetRadius;
                Vector3 targetPos = playerPos + new Vector3(offset.x, 0f, offset.y);

                // Spawn warning
                if (warningPrefab != null)
                {
                    Warning w = Instantiate(warningPrefab, targetPos, Quaternion.identity)
                        .GetComponent<Warning>();
                    w.Initialize(warningRadius, attackInterval, Warning.WarningType.Grounded, warningDuration);
                }

                // Spawn rock projectile
                if (rockPrefab != null && spawnTransform != null)
                {
                    GameObject rock = Instantiate(rockPrefab, spawnTransform.position, Quaternion.identity);
                    Rigidbody rb = rock.GetComponent<Rigidbody>();
                    if (rb == null)
                        rb = rock.AddComponent<Rigidbody>();

                    rb.isKinematic = true;
                    rb.useGravity = false;

                    boss.GetComponent<MonoBehaviour>().StartCoroutine(
                        LerpParabola(rock, rb, spawnTransform.position, targetPos, tossDuration)
                    );
                }

                yield return new WaitForSeconds(attackInterval);
            }
        }

        private IEnumerator LerpParabola(GameObject projectile, Rigidbody rb, Vector3 startPos, Vector3 targetPos, float duration)
        {
            float horizontalDistance = Vector3.Distance(
                new Vector3(startPos.x, 0, startPos.z), 
                new Vector3(targetPos.x, 0, targetPos.z)
            );
            float normalizedDistance = Mathf.Clamp01(horizontalDistance / targetRadius);
            float peakHeight = Mathf.Lerp(maxPeakHeight, minPeakHeight, normalizedDistance);
            
            // Calculate the apex point (highest point of the arc)
            float midY = Mathf.Max(startPos.y, targetPos.y) + peakHeight;
            
            float time = 0f;
            while (time < duration && projectile != null)
            {
                float t = time / duration;
                
                // Horizontal interpolation
                Vector3 horiz = Vector3.Lerp(startPos, targetPos, t);
                
                // Vertical parabolic arc using quadratic equation
                // At t=0: y = startPos.y
                // At t=0.5: y = midY (peak)
                // At t=1: y = targetPos.y
                float y = startPos.y + 
                          (midY - startPos.y) * 4 * t * (1 - t) + 
                          (targetPos.y - startPos.y) * t;
                
                Vector3 newPos = new Vector3(horiz.x, y, horiz.z);
                rb.MovePosition(newPos);

                time += Time.deltaTime;
                yield return null;
            }

            if (projectile != null)
                rb.MovePosition(targetPos);
        }

        public override void EndAttack(GameObject boss)
        {
            // Smoothly retract head over 1 second
            if (headTransform != null)
            {
                boss.GetComponent<MonoBehaviour>().StartCoroutine(RetractHead());
            }

            base.EndAttack(boss);
        }

        private IEnumerator RetractHead()
        {
            float t = 0f;
            Vector3 startPos = headTransform.localPosition;
            Quaternion startRot = headTransform.localRotation;
            
            while (t < 1f)
            {
                t += Time.deltaTime;
                headTransform.localPosition = Vector3.Lerp(startPos, headStartLocalPos, t);
                headTransform.localRotation = Quaternion.Lerp(startRot, headStartLocalRot, t);
                yield return null;
            }
            
            headTransform.localPosition = headStartLocalPos;
            headTransform.localRotation = headStartLocalRot;
        }
    }
}