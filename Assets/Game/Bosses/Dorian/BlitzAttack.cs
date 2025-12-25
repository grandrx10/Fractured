using System.Collections;
using System.Collections.Generic;
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
        public float delayBetweenDashes = 0.1f;      // now used as lerp duration
        public float delayBeforeDash = 0.5f;

        [Header("Warning Settings")]
        public GameObject warningPrefab;
        public int warningVisualRadius = 2;
        public float warningDuration = 0.5f;
        public float delayBetweenWarningSpawns = 0.2f;

        [Header("Line Settings")]
        public GameObject lineRendererPrefab;

        private LineRenderer lineRenderer;
        private List<Vector3> warningPositions = new List<Vector3>();

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            if (PlayerSingleton.Instance == null || lineRendererPrefab == null)
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

        private IEnumerator BlitzSequence(GameObject boss)
        {
            warningPositions.Clear();
            Vector3 prevPos = boss.transform.position;

            // -------------------------------
            // Phase 1: Spawn all warnings around the player's live position
            // -------------------------------
            for (int i = 0; i < warningCount && isActive; i++)
            {
                if (PlayerSingleton.Instance == null)
                    break;

                Vector3 playerPos = PlayerSingleton.Instance.GetPositionBelow();

                Vector3 warningPos;
                int attempts = 0;

                // Keep trying until the new point is far enough from previous
                do
                {
                    Vector2 offset = Random.insideUnitCircle * warningRadius;
                    warningPos = playerPos + new Vector3(offset.x, 0f, offset.y);
                    attempts++;
                    if (attempts > 100) break;
                } while (Vector3.Distance(warningPos, prevPos) < warningRadius);

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
            // Phase 2: Boss dashes to each warning smoothly
            // -------------------------------
            yield return new WaitForSeconds(delayBeforeDash);

            // Add boss starting position as the first line point
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

                    // Rotate boss to look at the target
                    Vector3 lookDir = (targetPos - boss.transform.position).normalized;
                    if (lookDir != Vector3.zero)
                        boss.transform.rotation = Quaternion.LookRotation(lookDir);

                    // Update line renderer
                    if (lineRenderer != null)
                    {
                        lineRenderer.positionCount = lineRenderer.positionCount < 2 ? 2 : lineRenderer.positionCount;
                        lineRenderer.SetPosition(lineRenderer.positionCount - 1, boss.transform.position);
                    }

                    yield return null;
                }

                // Ensure final position
                boss.transform.position = targetPos;
                if (lineRenderer != null)
                {
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, targetPos);
                }
            }

            // -------------------------------
            // Destroy line after 0.5s
            // -------------------------------
            if (lineRenderer != null)
            {
                yield return new WaitForSeconds(0.5f);
                Object.Destroy(lineRenderer.gameObject);
            }
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);
            // line destruction handled in coroutine
        }
    }
}
