using System.Collections;
using UnityEngine;
using Game.Bosses;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Ravenna
{
    [CreateAssetMenu(menuName = "BossAttacks/Ravenna/DeployScalesAttack")]
    public class DeployScalesAttack : BossAttack
    {
        [Header("Named Points")]
        public string arenaPointName;      // Where the scale ultimately lands
        public string dropTargetPointName; // Transform that will move downward
        public string scalePointName;      // Transform of the scale in the scene

        [Header("Prefabs")]
        public GameObject warningPrefab;

        [Header("Drop Settings")]
        public float spawnHeight = 200f;
        public float dropDuration = 3f;

        [Header("Post Drop Movement")]
        public float postDropOffset = 50f;
        public float postDropDuration = 1f;

        [Header("Warning Settings")]
        public int warningRadius = 6;
        public float warningDuration = 2f;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            // Get all the required transforms from the boss
            Transform arenaPoint = boss.GetComponent<Boss>().GetPointTransform(arenaPointName);
            Transform dropTarget = boss.GetComponent<Boss>().GetPointTransform(dropTargetPointName);
            Transform scale = boss.GetComponent<Boss>().GetPointTransform(scalePointName);

            if (arenaPoint == null || dropTarget == null || scale == null)
            {
                Debug.LogError("DeployScalesAttack: One or more named points not found.");
                EndAttack(boss);
                return;
            }

            boss.GetComponent<MonoBehaviour>()
                .StartCoroutine(DeployRoutine(boss, arenaPoint, dropTarget, scale));
        }

        private IEnumerator DeployRoutine(GameObject boss, Transform arenaPoint, Transform dropTarget, Transform scale)
        {
            Vector3 targetPos = arenaPoint.position;
            Vector3 spawnPos = targetPos + Vector3.up * spawnHeight;

            // Spawn warning
            if (warningPrefab != null)
            {
                Warning w = Instantiate(warningPrefab, targetPos, Quaternion.identity)
                    .GetComponent<Warning>();

                if (w != null)
                {
                    w.Initialize(
                        warningRadius,
                        warningDuration,
                        Warning.WarningType.Grounded,
                        warningDuration
                    );

                    Destroy(w.gameObject, warningDuration);
                }
            }

            yield return new WaitForSeconds(warningDuration);

            // Teleport the scale to spawn position and parent it to the arena point
            scale.position = spawnPos;
            scale.SetParent(arenaPoint, true); // keep world position

            // Lerp it down to the target position
            yield return LerpTransformPosition(
                scale,
                spawnPos,
                targetPos,
                dropDuration
            );

            // Lerp the drop target transform downward
            Vector3 start = dropTarget.position;
            Vector3 end = start - Vector3.up * postDropOffset;

            yield return LerpTransformPosition(
                dropTarget,
                start,
                end,
                postDropDuration
            );

            SetTrigger(boss, "next");
            EndAttack(boss);
        }

        private IEnumerator LerpTransformPosition(Transform t, Vector3 start, Vector3 end, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration && t != null)
            {
                float alpha = elapsed / duration;
                t.position = Vector3.Lerp(start, end, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (t != null)
                t.position = end;
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);
        }
    }
}
