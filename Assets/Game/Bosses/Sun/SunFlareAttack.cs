using System.Collections;
using Cards.Environments;
using Characters;
using UnityEngine;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Cyra
{
    [CreateAssetMenu(menuName = "BossAttacks/Cyra/SunFlareAttack")]
    public class SunFlareAttack : BossAttack
    {
        [Header("Settings")]
        public string spawnPointName;
        public string spawnPoint2Name;
        public float maxDistanceFromBoss = 20f;
        public float dnaSegmentLength = 20f;
        public float dnaHeight = 2f;
        public float dnaTwists = 3;

        public GameObject warningPrefab;
        public float warningRadius = 3f;
        public float warningDuration = 1f;

        public GameObject sunFlarePrefab;
        public LineRenderer linePrefab;

        [Header("Timing")]
        public float attackInterval = 2f;
        public float flareSpeed = 10f;
        public float warningStagger = 0.1f;
        public float lineDuration = 0.5f;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);
            
            boss.GetComponent<MonoBehaviour>()
                .StartCoroutine(ContinuousSunFlare(boss));
        }

        private IEnumerator ContinuousSunFlare(GameObject boss)
        {
            while (isActive)
            {
                boss.GetComponent<MonoBehaviour>()
                    .StartCoroutine(SpawnSunFlare(boss));

                yield return new WaitForSeconds(attackInterval);
            }
        }

        private IEnumerator SpawnSunFlare(GameObject boss)
        {
            Vector3 playerPos = OpenWorldEnv.Current.GetBossTargetGrounded();

            // Boss hand / spawn point
            Transform spawnPoint = boss.GetComponent<Boss>()
                .GetPointTransform(spawnPointName);
            Transform spawnPoint2 = boss.GetComponent<Boss>()
                .GetPointTransform(spawnPoint2Name);

            Vector3 bossPos = boss.transform.position;

            // Direction boss -> player (flattened)
            Vector3 toPlayer = playerPos - bossPos;
            toPlayer.y = 0f;
            toPlayer.Normalize();

            // Pick distance BETWEEN boss and player
            float forwardDistance = Random.Range(0f, maxDistanceFromBoss);

            // Small sideways randomness
            Vector3 sideways = Vector3.Cross(Vector3.up, toPlayer).normalized;
            float sidewaysOffset = Random.Range(
                -maxDistanceFromBoss * 0.3f,
                maxDistanceFromBoss * 0.3f
            );

            // Spawn above ground so we can raycast down
            Vector3 tentativePoint =
                bossPos
                + toPlayer * forwardDistance
                + sideways * sidewaysOffset
                + Vector3.up * 10f;

            Vector3 startPoint;
            if (Physics.Raycast(
                tentativePoint,
                Vector3.down,
                out RaycastHit hit,
                100f,
                LayerMask.GetMask("Ground")))
            {
                startPoint = hit.point;
            }
            else
            {
                startPoint = tentativePoint;
            }

            // End point continues toward the player
            Vector3 endPoint = startPoint + toPlayer * dnaSegmentLength;
            endPoint.y = startPoint.y;

            // Spawn warnings
            if (warningPrefab != null)
            {
                float lineLength = Vector3.Distance(startPoint, endPoint);
                int warningCount = Mathf.CeilToInt(lineLength / warningRadius);

                for (int i = 0; i <= warningCount; i++)
                {
                    float t = i / (float)warningCount;
                    Vector3 warningPos = Vector3.Lerp(startPoint, endPoint, t);

                    Warning w = Instantiate(
                        warningPrefab,
                        warningPos,
                        Quaternion.identity
                    ).GetComponent<Warning>();

                    float staggeredDuration = warningDuration + i * warningStagger;
                    w.Initialize(
                        warningRadius,
                        0.5f,
                        Warning.WarningType.Grounded,
                        staggeredDuration
                    );
                }
            }

            yield return new WaitForSeconds(warningDuration);

            // Spawn flares
            GameObject flare1 = Instantiate(sunFlarePrefab, startPoint, Quaternion.identity);
            GameObject flare2 = Instantiate(sunFlarePrefab, startPoint, Quaternion.identity);

            // Lines (one per flare)
            LineRenderer line1 = null;
            LineRenderer line2 = null;

            if (linePrefab != null)
            {
                line1 = Instantiate(linePrefab);
                line1.positionCount = 2;

                line2 = Instantiate(linePrefab);
                line2.positionCount = 2;
            }

            float totalDistance = Vector3.Distance(startPoint, endPoint);
            float duration = totalDistance / flareSpeed;
            float time = 0f;

            Vector3 perpendicular = Vector3.Cross(Vector3.up, toPlayer).normalized;

            while (time < duration)
            {
                float t = time / duration;
                Vector3 linePos = Vector3.Lerp(startPoint, endPoint, t);

                float sine =
                    Mathf.Sin(t * dnaTwists * Mathf.PI * 2f) * dnaHeight;

                Vector3 pos1 = linePos + perpendicular * sine * 0.5f;
                Vector3 pos2 = linePos - perpendicular * sine * 0.5f;

                flare1.transform.position = pos1;
                flare2.transform.position = pos2;

                if (line1 != null)
                {
                    line1.SetPosition(0, spawnPoint.position);
                    line1.SetPosition(1, pos1);
                }

                if (line2 != null)
                {
                    line2.SetPosition(0, spawnPoint2.position);
                    line2.SetPosition(1, pos2);
                }

                time += Time.deltaTime;
                yield return null;
            }

            flare1.transform.position = endPoint;
            flare2.transform.position = endPoint;

            Destroy(flare1);
            Destroy(flare2);

            if (line1 != null) Destroy(line1.gameObject, lineDuration);
            if (line2 != null) Destroy(line2.gameObject, lineDuration);
        }
    }
}
