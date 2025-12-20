using System.Collections;
using Characters;
using UnityEngine;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Cyra
{
    [CreateAssetMenu(menuName = "BossAttacks/Cyra/SunFlareAttack")]
    public class SunFlareAttack : BossAttack
    {
        [Header("Settings")]
        public string spawnPointName;           // Point on boss to start the line (like hand)
        public float maxDistanceFromBoss = 20f;
        public float dnaSegmentLength = 20f;
        public float dnaHeight = 2f;            // height of the sine wave
        public int dnaTwists = 3;               // number of twists along the line
        public GameObject warningPrefab;
        public float warningRadius = 3f;
        public float warningDuration = 1f;
        public GameObject sunFlarePrefab;
        public LineRenderer linePrefab;         // assign a LineRenderer prefab

        [Header("Timing")]
        public float attackInterval = 2f;       // seconds between each new flare attack
        public float flareSpeed = 10f;          // units per second along the DNA path
        public float warningStagger = 0.1f;     // delay between warnings along the line
        public float lineDuration = 0.5f;       // duration of the line effect

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            if (PlayerSingleton.Instance != null)
            {
                boss.GetComponent<MonoBehaviour>().StartCoroutine(ContinuousSunFlare(boss));
            }
        }

        private IEnumerator ContinuousSunFlare(GameObject boss)
        {
            while (isActive && PlayerSingleton.Instance != null)
            {
                boss.GetComponent<MonoBehaviour>().StartCoroutine(SpawnSunFlare(boss));
                yield return new WaitForSeconds(attackInterval);
            }
        }

        private IEnumerator SpawnSunFlare(GameObject boss)
        {
            Vector3 playerPos = PlayerSingleton.Instance.GetPositionBelow();

            // Get spawn point on the boss (like hand)
            Transform spawnPoint = boss.GetComponent<Boss>().GetPointTransform(spawnPointName);

            if (spawnPoint == null)
            {
                Debug.LogWarning($"SunFlareAttack: spawn point '{spawnPointName}' not found on boss. Using boss position instead.");
                spawnPoint = boss.transform;
            }

            // 1) Pick a random point near the boss for DNA start, on the ground
            Vector2 randomOffset = Random.insideUnitCircle * maxDistanceFromBoss;
            Vector3 tentativePoint = boss.transform.position + new Vector3(randomOffset.x, 10f, randomOffset.y); // above boss
            Vector3 startPoint;

            // Raycast down to find the ground
            if (Physics.Raycast(tentativePoint, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            {
                startPoint = hit.point; // grounded start
            }
            else
            {
                // fallback if no ground found
                startPoint = new Vector3(tentativePoint.x, boss.transform.position.y, tentativePoint.z);
            }
            // 2) Find end point along the line toward the player, same y as startPoint
            Vector3 dirToPlayer = (playerPos - startPoint).normalized;
            Vector3 endPoint = startPoint + dirToPlayer * dnaSegmentLength;
            endPoint.y = startPoint.y; // same ground level

            // 3) Spawn warnings along the line every 'warningRadius' units, staggered
            if (warningPrefab != null)
            {
                float lineLength = Vector3.Distance(startPoint, endPoint);
                int warningCount = Mathf.CeilToInt(lineLength / warningRadius);

                for (int i = 0; i <= warningCount; i++)
                {
                    float t = i / (float)warningCount;
                    Vector3 warningPos = Vector3.Lerp(startPoint, endPoint, t);

                    Warning w = Instantiate(warningPrefab, warningPos, Quaternion.identity)
                        .GetComponent<Warning>();

                    float staggeredDuration = warningDuration + i * warningStagger;
                    w.Initialize(warningRadius, 0.5f, Warning.WarningType.Grounded, staggeredDuration);
                }
            }

            yield return new WaitForSeconds(warningDuration);

            // 4) Spawn 2 sun flare objects at start
            GameObject flare1 = Instantiate(sunFlarePrefab, startPoint, Quaternion.identity);
            GameObject flare2 = Instantiate(sunFlarePrefab, startPoint, Quaternion.identity);

            // 5) Draw a line from boss hand to DNA start point when flares spawn
            if (linePrefab != null)
            {
                LineRenderer line = Instantiate(linePrefab);
                line.positionCount = 2;
                line.SetPosition(0, spawnPoint.position); // boss hand
                line.SetPosition(1, startPoint);          // DNA start
                Destroy(line.gameObject, lineDuration);
            }

            // 6) Move the 2 objects along the line in a DNA pattern
            float totalDistance = Vector3.Distance(startPoint, endPoint);
            float duration = totalDistance / flareSpeed;
            float time = 0f;

            while (time < duration)
            {
                float t = time / duration;
                Vector3 linePos = Vector3.Lerp(startPoint, endPoint, t);

                float sine = Mathf.Sin(t * dnaTwists * Mathf.PI * 2f) * dnaHeight;

                Vector3 perpendicular = Vector3.Cross(Vector3.up, dirToPlayer).normalized;

                flare1.transform.position = linePos + perpendicular * sine * 0.5f;
                flare2.transform.position = linePos - perpendicular * sine * 0.5f;

                time += Time.deltaTime;
                yield return null;
            }

            // Ensure they reach the end point
            flare1.transform.position = endPoint;
            flare2.transform.position = endPoint;
            Destroy(flare1);
            Destroy(flare2);
        }
    }
}
