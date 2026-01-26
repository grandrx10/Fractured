using System.Collections;
using System.Collections.Generic;
using Cards.Environments;
using Characters;
using UnityEngine;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Dorian
{
    [CreateAssetMenu(menuName = "BossAttacks/Dorian/BlitzAttack")]
    public class BlitzAttack : BossAttack
    {
        [Header("Blitz Settings")]
        public int warningCount = 10;
        public float warningRadius = 30f;
        public float heightAboveWarning = 2.5f;
        public float delayBetweenDashes = 0.1f;
        public float delayBeforeDash = 0.5f;

        [Header("Warning Settings")]
        public GameObject warningPrefab;
        public int warningVisualRadius = 2;
        public float warningDuration = 0.5f;
        public float delayBetweenWarningSpawns = 0.2f;

        [Header("Line Settings")]
        public GameObject lineRendererPrefab;

        protected LineRenderer lineRenderer;
        protected List<Vector3> warningPositions = new List<Vector3>();

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            if (lineRendererPrefab == null)
                return;

            GameObject lineObj = Object.Instantiate(lineRendererPrefab);
            lineRenderer = lineObj.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogError("LineRenderer prefab does not have a LineRenderer component.");
                return;
            }

            lineRenderer.positionCount = 0;
            lineRenderer.useWorldSpace = true;

            boss.GetComponent<MonoBehaviour>()
                .StartCoroutine(BlitzSequence(boss));
        }

        protected virtual IEnumerator BlitzSequence(GameObject boss)
        {
            warningPositions.Clear();
            Vector3 prevPos = boss.transform.position;

            // -------------------------------
            // Phase 1: Spawn warnings
            // -------------------------------
            for (int i = 0; i < warningCount && isActive; i++)
            {
                Vector3 playerPos = OpenWorldEnv.Current.GetBossTargetGrounded();
                Vector3 warningPos;
                int attempts = 0;

                do
                {
                    Vector2 offset = Random.insideUnitCircle * warningRadius;
                    warningPos = playerPos + new Vector3(offset.x, 0f, offset.y);
                    attempts++;
                    if (attempts > 100) break;
                }
                while (Vector3.Distance(warningPos, prevPos) < warningRadius);

                warningPositions.Add(warningPos);
                prevPos = warningPos;

                if (warningPrefab != null)
                {
                    Warning w = Object.Instantiate(
                        warningPrefab,
                        warningPos,
                        Quaternion.identity
                    ).GetComponent<Warning>();

                    w.Initialize(
                        warningVisualRadius,
                        warningDuration,
                        Warning.WarningType.Grounded,
                        warningDuration
                    );
                }

                yield return new WaitForSeconds(delayBetweenWarningSpawns);
            }

            // -------------------------------
            // Phase 2: Dash sequence
            // -------------------------------
            yield return new WaitForSeconds(delayBeforeDash);

            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 1;
                lineRenderer.SetPosition(0, boss.transform.position);
            }

            foreach (var pos in warningPositions)
            {
                if (!isActive)
                    break;

                Vector3 targetPos = pos + Vector3.up * heightAboveWarning;
                Vector3 startPos = boss.transform.position;
                float elapsed = 0f;

                while (elapsed < delayBetweenDashes && isActive)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / delayBetweenDashes);
                    boss.transform.position = Vector3.Lerp(startPos, targetPos, t);

                    Vector3 lookDir = (targetPos - boss.transform.position).normalized;
                    if (lookDir != Vector3.zero)
                        boss.transform.rotation = Quaternion.LookRotation(lookDir);

                    if (lineRenderer != null)
                    {
                        lineRenderer.positionCount = Mathf.Max(2, lineRenderer.positionCount);
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, boss.transform.position);
                    }

                    yield return null;
                }

                // Finalize dash
                boss.transform.position = targetPos;

                if (lineRenderer != null)
                {
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, targetPos);
                }

                // 🔹 EXTENSION POINT
                OnDashComplete(boss);
            }

            // -------------------------------
            // Cleanup line
            // -------------------------------
            if (lineRenderer != null)
            {
                yield return new WaitForSeconds(0.5f);
                Object.Destroy(lineRenderer.gameObject);
            }
        }

        protected virtual void OnDashComplete(GameObject boss)
        {
            // Intentionally empty — overridden by derived attacks
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);
        }
    }
}
