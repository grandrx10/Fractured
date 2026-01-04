using Cards.Environments;
using UnityEngine;

namespace Game.Bosses.Rock
{
    [CreateAssetMenu(menuName = "BossAttacks/Rock/RollAttack")]
    public class RollAttack : BossAttack
    {
        [Header("Movement")]
        public float startSpeed = 3f;
        public float maxSpeed = 12f;
        public float turnSpeed = 5f;

        [Header("Rolling")]
        public string rollPointName = "RollRoot";
        public float rollDegreesPerMeter = 360f;

        [Header("Raycast Settings")]
        public LayerMask groundMask; // layers considered "ground"
        public float rayDistance = 2f; // distance to check forward

        private float currentSpeed;
        private float elapsed;
        private float lockedY;

        private Transform bossTransform;
        private Transform playerTransform;
        private Transform rollTransform;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            bossTransform = boss.transform;
            playerTransform = OpenWorldEnv.Current.PlayerTransform;

            Boss bossComp = boss.GetComponent<Boss>();
            if (bossComp != null)
                rollTransform = bossComp.GetPointTransform(rollPointName);

            elapsed = 0f;
            currentSpeed = startSpeed;
            lockedY = bossTransform.position.y;
        }

        public override void Tick(GameObject boss)
        {
            if (playerTransform == null)
                return;

            elapsed += Time.deltaTime;

            // =========================
            // SPEED PROFILE
            // =========================
            float t = Mathf.Clamp01(elapsed / attackDuration);
            if (t <= 0.75f)
                currentSpeed = Mathf.Lerp(startSpeed, maxSpeed, t / 0.75f);
            else
                currentSpeed = Mathf.Lerp(maxSpeed, 0f, (t - 0.75f) / 0.25f);

            currentSpeed = Mathf.Max(0f, currentSpeed);

            // =========================
            // ROTATION TOWARD PLAYER
            // =========================
            Vector3 toPlayer = playerTransform.position - bossTransform.position;
            toPlayer.y = 0f;

            if (toPlayer.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(toPlayer.normalized);
                
                // Check if hitting a wall ahead
                RaycastHit hit;
                bool hitWall = Physics.Raycast(bossTransform.position, bossTransform.forward, out hit, rayDistance, groundMask);
                
                if (hitWall)
                {
                    // Instant turn when hitting wall
                    bossTransform.rotation = targetRotation;
                }
                else
                {
                    // Smooth turn during normal movement
                    bossTransform.rotation = Quaternion.RotateTowards(
                        bossTransform.rotation, 
                        targetRotation, 
                        turnSpeed * Time.deltaTime
                    );
                }
            }

            // =========================
            // MOVEMENT
            // =========================
            Vector3 movement = bossTransform.forward * currentSpeed * Time.deltaTime;
            bossTransform.position += movement;
            
            // Lock Y position
            bossTransform.position = new Vector3(
                bossTransform.position.x, 
                lockedY, 
                bossTransform.position.z
            );

            // =========================
            // VISUAL ROLLING
            // =========================
            if (rollTransform != null && currentSpeed > 0.01f)
            {
                float distanceMoved = currentSpeed * Time.deltaTime;
                float rotationDegrees = distanceMoved * rollDegreesPerMeter;
                rollTransform.Rotate(Vector3.right, rotationDegrees, Space.Self);
            }
        }
    }
}