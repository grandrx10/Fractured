using Cards.Environments;
using UnityEngine;
using Game.Bosses.Projectiles;

namespace Game.Minions
{
    public class ChargeMinionAttack : MinionAttack
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

        [Header("Warning")]
        public Warning warningPrefab;
        public float warningWidth = 3f;
        public float warningDuration = 0.75f;
        public float warningFadeTime = 0.2f;

        [Header("Collision")]
        public LayerMask wallMask;
        public float rayDistance = 1f;

        [Header("Random Offset")]
        public float radiusXZ = 3f; // random offset radius around player

        private float elapsed;
        private float chargeElapsed;

        private Vector3 lockedDirection;
        private Vector3 targetOffsetPosition;
        private float lockedY;
        private bool warningSpawned;

        public override void Activate()
        {
            base.Activate();

            elapsed = 0f;
            chargeElapsed = 0f;
            lockedDirection = Vector3.zero;
            warningSpawned = false;

            // pick a random XZ offset from the player
            Vector2 randomCircle = Random.insideUnitCircle * radiusXZ;
            Vector3 playerPos = playerTransform.position;
            targetOffsetPosition = new Vector3(
                playerPos.x + randomCircle.x,
                playerPos.y,
                playerPos.z + randomCircle.y
            );

            lockedY = minionTransform.position.y;
        }

        public override void Tick()
        {
            if (!Active || playerTransform == null)
                return;

            elapsed += Time.deltaTime;

            // =========================
            // FACE TARGET (CHARGE-UP)
            // =========================
            if (elapsed < faceDuration)
            {
                Vector3 toTarget = targetOffsetPosition - minionTransform.position;
                toTarget.y = 0f;

                if (toTarget.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized);
                    minionTransform.rotation = Quaternion.Slerp(
                        minionTransform.rotation,
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
                Vector3 toTarget = targetOffsetPosition - minionTransform.position;
                toTarget.y = 0f;
                lockedDirection = toTarget.normalized;
                chargeElapsed = 0f;
                SpawnWarning();
            }

            chargeElapsed += Time.deltaTime;

            float chargeDuration = duration - faceDuration;
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
                speed = Mathf.Lerp(
                    maxSpeed,
                    0f,
                    (t - (1f - decelPortion)) / Mathf.Max(decelPortion, 0.0001f)
                );
            }
            else
            {
                speed = maxSpeed;
            }

            // =========================
            // MOVE STRAIGHT WITH WALL CHECK
            // =========================
            float distance = speed * Time.deltaTime;

            if (Physics.Raycast(
                minionTransform.position,
                lockedDirection,
                out _,
                distance + rayDistance,
                wallMask))
            {
                distance = 0f;
            }

            Vector3 newPos = minionTransform.position + lockedDirection * distance;
            newPos.y = lockedY;
            minionTransform.position = newPos;
        }

        private void SpawnWarning()
        {
            if (warningSpawned || warningPrefab == null)
                return;

            warningSpawned = true;

            float chargeDuration = Mathf.Max(0f, duration - faceDuration);
            float estimatedDistance = maxSpeed * chargeDuration * 0.8f;

            Vector3 centerOffset = lockedDirection * (estimatedDistance * 0.5f);
            Vector3 spawnPos = minionTransform.position + centerOffset;

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
    }
}
