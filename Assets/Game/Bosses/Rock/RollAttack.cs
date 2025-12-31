using UnityEngine;
using Characters;
using Game.Bosses;

namespace Game.Bosses.Rock
{
    [CreateAssetMenu(menuName = "BossAttacks/Rock/RollAttack")]
    public class RollAttack : BossAttack
    {
        [Header("Movement")]
        public float startSpeed = 3f;
        public float maxSpeed = 12f;
        public float turnSpeed = 5f;

        private float currentSpeed;
        private float elapsed;
        private float lockedY;

        private Transform bossTransform;
        private Transform playerTransform;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            bossTransform = boss.transform;
            playerTransform = PlayerSingleton.Instance.transform;

            elapsed = 0f;
            currentSpeed = startSpeed;

            // Lock Y level at attack start
            lockedY = bossTransform.position.y;
        }

        public override void Tick(GameObject boss)
        {
            if (playerTransform == null)
                return;

            elapsed += Time.deltaTime;

            // =========================
            // Turn toward player (Y-only)
            // =========================
            Vector3 toPlayer = playerTransform.position - bossTransform.position;
            toPlayer.y = 0f;

            if (toPlayer.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized);
                bossTransform.rotation = Quaternion.Slerp(
                    bossTransform.rotation,
                    targetRot,
                    turnSpeed * Time.deltaTime
                );
            }

            // =========================
            // Speed profile
            // Peak at 75%, stop at end
            // =========================
            float normalizedTime = Mathf.Clamp01(elapsed / attackDuration);

            if (normalizedTime <= 0.75f)
            {
                // Accelerate to max speed
                float t = normalizedTime / 0.75f;
                currentSpeed = Mathf.Lerp(startSpeed, maxSpeed, t);
            }
            else
            {
                // Decelerate to full stop
                float t = (normalizedTime - 0.75f) / 0.25f;
                currentSpeed = Mathf.Lerp(maxSpeed, 0f, t);
            }

            currentSpeed = Mathf.Max(0f, currentSpeed);

            // =========================
            // Move forward
            // =========================
            Vector3 move = bossTransform.forward * currentSpeed * Time.deltaTime;
            Vector3 newPos = bossTransform.position + move;

            // Lock Y
            newPos.y = lockedY;
            bossTransform.position = newPos;
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);
        }
    }
}
