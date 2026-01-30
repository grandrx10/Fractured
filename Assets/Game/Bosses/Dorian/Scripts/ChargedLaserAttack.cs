using System.Collections;
using Cards.Environments;
using UnityEngine;
using Characters;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Dorian
{
    [CreateAssetMenu(menuName = "BossAttacks/Dorian/ChargedLaser")]
    public class ChargedLaser : BossAttack
    {
        [Header("Prefabs")]
        public GameObject warningPrefab;  // visual cue on the hand
        public GameObject laserPrefab;    // actual laser

        [Header("Hand Settings")]
        public string handPointName;      // boss point to attach warning & laser to

        [Header("Timing Settings")]
        public float initialRotationTime = 0.3f; // time to quickly rotate at start
        public float chargeDelay = 1.5f;         // delay before laser fires
        public float laserDuration = 3f;         // how long the laser stays active

        [Header("Rotation Settings")]
        public float rotationSpeed, rotationSpeedStart = 90f; // degrees per second toward the player during charge/laser

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);
            MonoBehaviour mb = boss.GetComponent<MonoBehaviour>();
            if (mb != null)
                mb.StartCoroutine(LaserSequence(boss));
            else
                Debug.LogError("Boss has no MonoBehaviour component!");
        }

        private IEnumerator LaserSequence(GameObject boss)
        {
            // Get hand transform
            Transform hand = boss.GetComponent<Boss>()?.GetPointTransform(handPointName);
            if (hand == null)
                hand = boss.transform;

            // --------------------------
            // Phase 0: Fast initial rotation (~0.3s)
            // --------------------------
            if (true)
            {
                float elapsed = 0f;
                Quaternion startRot = boss.transform.rotation;
                Vector3 dir = (OpenWorldEnv.Current.GetBossTargetGrounded() - boss.transform.position).normalized;
                Quaternion targetRot = dir != Vector3.zero ? Quaternion.LookRotation(dir) : boss.transform.rotation;

                while (elapsed < initialRotationTime && isActive)
                {
                    elapsed += Time.deltaTime;
                    boss.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / initialRotationTime);
                    yield return null;
                }
                boss.transform.rotation = targetRot; // ensure exact rotation at end
            }

            // --------------------------
            // Spawn warning on hand (visual only)
            // --------------------------
            GameObject warningObj = null;
            if (warningPrefab != null)
            {
                warningObj = Object.Instantiate(warningPrefab, hand.position, hand.rotation, hand);
            }

            // --------------------------
            // Phase 1: Charge delay while rotating at rotationSpeed
            // --------------------------
            float chargeElapsed = 0f;
            while (chargeElapsed < chargeDelay && isActive)
            {
                Vector3 playerPos = OpenWorldEnv.Current.PlayerPos;
                Vector3 dir = (playerPos - boss.transform.position).normalized;
                if (dir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    boss.transform.rotation = Quaternion.RotateTowards(
                        boss.transform.rotation,
                        targetRot,
                        rotationSpeedStart * Time.deltaTime
                    );
                }

                chargeElapsed += Time.deltaTime;
                yield return null;
            }

            // Destroy warning
            if (warningObj != null)
                Object.Destroy(warningObj);

            // --------------------------
            // Phase 2: Spawn laser and keep rotating toward player
            // --------------------------
            GameObject laserObj = null;
            if (laserPrefab != null)
            {
                laserObj = Object.Instantiate(laserPrefab, hand.position, hand.rotation, hand);
            }

            float laserElapsed = 0f;
            while (laserElapsed < laserDuration && isActive)
            {
                Vector3 playerPos = OpenWorldEnv.Current.PlayerPos;
                Vector3 dir = (playerPos - boss.transform.position).normalized;
                if (dir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    boss.transform.rotation = Quaternion.RotateTowards(
                        boss.transform.rotation,
                        targetRot,
                        rotationSpeed * Time.deltaTime
                    );
                }

                laserElapsed += Time.deltaTime;
                yield return null;
            }

            // Destroy laser
            if (laserObj != null)
                Object.Destroy(laserObj);
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);
            // Laser destruction handled in coroutine
        }
    }
}
