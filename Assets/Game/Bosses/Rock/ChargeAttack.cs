using Cards.Environments;
using UnityEngine;
using Game.Bosses.Projectiles;

namespace Game.Bosses
{
    [CreateAssetMenu(menuName = "BossAttacks/Rock/ChargeAttack")]
    public class ChargeAttack : BossAttack
    {
        [Header("Charge-Up Facing")]
        public float faceDuration = 0.75f;
        public float faceTurnSpeed = 6f;

        [Header("Charge Movement")]
        public float maxSpeed = 14f;
        [Tooltip("Portion of charge time spent accelerating (0–0.5)")]
        public float accelPortion = 0.2f;
        [Tooltip("Portion of charge time spent decelerating (0–0.5)")]
        public float decelPortion = 0.2f;

        [Header("Rolling")]
        public string rollPointName = "RollRoot";
        public float rollDegreesPerMeter = 360f;

        [Header("Warning")]
        public Warning warningPrefab;
        public float warningWidth = 3f;
        public float warningDuration = 0.75f;
        public float warningFadeTime = 0.2f;

        [Header("Collision")]
        public LayerMask wallMask;     // layers considered "walls"
        public float rayDistance = 1f; // forward raycast distance to detect walls

        private Transform bossTransform;
        private Transform playerTransform;
        private Transform rollTransform;

        private float elapsed;
        private float chargeElapsed;

        private Vector3 lockedDirection;
        private float lockedY;
        private bool warningSpawned;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            bossTransform = boss.transform;
            playerTransform = OpenWorldEnv.Current.PlayerTransform;

            Boss bossComp = boss.GetComponent<Boss>();
            if (bossComp != null)
                rollTransform = bossComp.GetPointTransform(rollPointName);

            elapsed = 0f;
            chargeElapsed = 0f;
            lockedDirection = Vector3.zero;
            warningSpawned = false;

            lockedY = bossTransform.position.y;
        }

        public override void Tick(GameObject boss)
        {
            if (playerTransform == null)
                return;

            elapsed += Time.deltaTime;

            // =========================
            // FACE PLAYER (CHARGE-UP)
            // =========================
            if (elapsed < faceDuration)
            {
                Vector3 toPlayer = playerTransform.position - bossTransform.position;
                toPlayer.y = 0f;

                if (toPlayer.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized);
                    bossTransform.rotation = Quaternion.Slerp(
                        bossTransform.rotation,
                        targetRot,
                        faceTurnSpeed * Time.deltaTime
                    );
                }
                return;
            }

            // =========================
            // LOCK DIRECTION ONCE
            // =========================
            if (lockedDirection == Vector3.zero)
            {
                lockedDirection = bossTransform.forward.normalized;
                chargeElapsed = 0f;
                SpawnWarning();
            }

            chargeElapsed += Time.deltaTime;

            float chargeDuration = attackDuration - faceDuration;
            if (chargeDuration <= 0f)
                return;

            float t = Mathf.Clamp01(chargeElapsed / chargeDuration);

            // =========================
            // SPEED CURVE
            // =========================
            float speed;
            if (t < accelPortion)
            {
                speed = Mathf.Lerp(0f, maxSpeed, t / Mathf.Max(accelPortion, 0.0001f));
            }
            else if (t > 1f - decelPortion)
            {
                speed = Mathf.Lerp(maxSpeed, 0f, (t - (1f - decelPortion)) / Mathf.Max(decelPortion, 0.0001f));
            }
            else
            {
                speed = maxSpeed;
            }

            // =========================
            // MOVE STRAIGHT WITH WALL CHECK (STOP AT WALL)
            // =========================
            float distance = speed * Time.deltaTime;

            // Raycast forward to detect wall
            if (Physics.Raycast(bossTransform.position, lockedDirection, out RaycastHit hit, distance + rayDistance, wallMask))
            {
                // Wall detected: stop movement for this frame
                distance = 0f;
            }

            // Move boss smoothly (distance may be zero if wall detected)
            Vector3 newPos = bossTransform.position + lockedDirection * distance;
            newPos.y = lockedY;
            bossTransform.position = newPos;



            // =========================
            // ROLL VISUAL
            // =========================
            if (rollTransform != null && distance > 0f)
            {
                rollTransform.Rotate(
                    Vector3.right,
                    distance * rollDegreesPerMeter,
                    Space.Self
                );
            }
        }

        private void SpawnWarning()
        {
            if (warningSpawned || warningPrefab == null)
                return;

            warningSpawned = true;

            float chargeDuration = Mathf.Max(0f, attackDuration - faceDuration);
            float estimatedDistance = maxSpeed * chargeDuration * 0.8f;

            Vector3 centerOffset = lockedDirection * (estimatedDistance * 0.5f);
            Vector3 spawnPos = bossTransform.position + centerOffset;

            Quaternion rot = Quaternion.LookRotation(lockedDirection);

            Warning w = Object.Instantiate(
                warningPrefab,
                spawnPos,
                rot
            );

            w.InitializeRectangle(
                new Vector2(warningWidth, estimatedDistance),
                warningDuration,
                Warning.WarningType.Grounded,
                warningFadeTime
            );
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);
        }
    }
}
