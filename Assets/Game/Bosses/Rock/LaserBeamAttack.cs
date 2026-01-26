using Cards.Environments;
using UnityEngine;
using Characters;
using Game.Bosses;

namespace Game.Bosses.Rock
{
    [CreateAssetMenu(menuName = "BossAttacks/Rock/LaserBeamAttack")]
    public class LaserBeamAttack : BossAttack
    {
        [Header("Named Points")]
        public string headPointName = "Head";
        public string headOutPointName = "HeadOut";
        public string laserPointName = "LaserPoint";

        [Header("Laser")]
        public GameObject laserPrefab;

        [Header("Timing")]
        public float extendTime = 0.5f;
        public float retractTime = 1f; // now fixed 1 second
        public float laserOffsetBeforeEnd = 2f; // despawn laser 1 sec before attack ends

        [Header("Turning")]
        public float initialTurnSpeed = 6f; // During windup
        public float laserTurnSpeed = 2f;   // While laser is active

        private Transform bossTransform;
        private Transform playerTransform;

        private Transform headTransform;
        private Transform headOutTransform;
        private Transform laserTransform;

        private Vector3 headStartLocalPos;
        private GameObject activeLaser;

        private float elapsed;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            bossTransform = boss.transform;
            playerTransform = OpenWorldEnv.Current.PlayerTransform;

            Boss bossComp = boss.GetComponent<Boss>();

            headTransform = bossComp.GetPointTransform(headPointName);
            headOutTransform = bossComp.GetPointTransform(headOutPointName);
            laserTransform = bossComp.GetPointTransform(laserPointName);

            if (headTransform != null)
                headStartLocalPos = headTransform.localPosition;

            elapsed = 0f;
            activeLaser = null;
        }

        public override void Tick(GameObject boss)
        {
            if (playerTransform == null || headTransform == null)
            {
                Debug.LogWarning("Missing transforms");
                return;
            }

            elapsed += Time.deltaTime;

            // =========================
            // Determine attack phase
            // =========================
            bool extending = elapsed <= extendTime;
            bool retracting = elapsed >= attackDuration - retractTime;
            // Laser is active from extendTime until attackDuration - laserOffsetBeforeEnd
            bool laserActive = elapsed >= extendTime && elapsed <= attackDuration - laserOffsetBeforeEnd;

            // =========================
            // Turn boss toward player (Y-only)
            // =========================
            Vector3 toPlayer = playerTransform.position - bossTransform.position;
            toPlayer.y = 0f;

            if (toPlayer.sqrMagnitude > 0.001f)
            {
                float turnSpeed = laserActive ? laserTurnSpeed : initialTurnSpeed;
                Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized);
                bossTransform.rotation = Quaternion.Slerp(
                    bossTransform.rotation,
                    targetRot,
                    turnSpeed * Time.deltaTime
                );
            }

            // =========================
            // Tilt head toward player (pitch + yaw)
            // =========================
            Vector3 headToPlayer = playerTransform.position - headTransform.position;
            if (headToPlayer.sqrMagnitude > 0.001f)
            {
                Quaternion headTargetRot = Quaternion.LookRotation(headToPlayer);
                float headTurnSpeed = laserActive ? laserTurnSpeed : initialTurnSpeed;
                headTransform.rotation = Quaternion.Slerp(
                    headTransform.rotation,
                    headTargetRot,
                    headTurnSpeed * Time.deltaTime
                );
            }

            // =========================
            // Head movement (extend/retract)
            // =========================
            if (extending && headOutTransform != null)
            {
                float t = Mathf.Clamp01(elapsed / extendTime);
                headTransform.position = Vector3.Lerp(
                    headTransform.position,
                    headOutTransform.position,
                    t
                );
            }
            else if (retracting)
            {
                // Retract smoothly over retractTime
                float t = Mathf.Clamp01(
                    (elapsed - (attackDuration - retractTime)) / retractTime
                );

                headTransform.localPosition = Vector3.Lerp(
                    headTransform.localPosition,
                    headStartLocalPos,
                    t
                );
            }

            // =========================
            // Spawn laser
            // =========================
            if (laserActive && activeLaser == null && laserPrefab != null && laserTransform != null)
            {
                activeLaser = Instantiate(
                    laserPrefab,
                    laserTransform.position,
                    laserTransform.rotation,
                    laserTransform // parent at spawn
                );
            }

            // Destroy laser 1 second before end
            if (activeLaser != null && elapsed > attackDuration - laserOffsetBeforeEnd)
            {
                Destroy(activeLaser);
                activeLaser = null;
            }
        }

        public override void EndAttack(GameObject boss)
        {
            // Ensure laser is destroyed
            if (activeLaser != null)
            {
                Destroy(activeLaser);
                activeLaser = null;
            }

            // Reset head immediately at end (in case tick missed last frame)
            if (headTransform != null)
                headTransform.localPosition = headStartLocalPos;

            base.EndAttack(boss);
        }
    }
}
