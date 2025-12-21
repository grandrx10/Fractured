using System.Collections;
using Characters;
using UnityEngine;

namespace Game.Bosses.Cyra
{
    [CreateAssetMenu(menuName = "BossAttacks/Cyra/LaserWallAttack")]
    public class LaserWallAttack : BossAttack
    {
        [Header("Boss Points")]
        public string startPointName;   // where the first row starts
        public string endPointName;     // where the attack stops

        [Header("Laser Wall Settings")]
        public GameObject laserWallPrefab;
        public int wallsPerRow = 5;
        public float wallSpacing = 2f;     // spacing sideways
        public float rowSpacing = 4f;      // distance between rows

        [Header("Timing")]
        public float rowInterval = 4f;     // seconds between rows

        private Transform startPoint;
        private Transform endPoint;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            Boss bossComp = boss.GetComponent<Boss>();
            if (bossComp == null)
            {
                Debug.LogError("LaserWallAttack: Boss component missing.");
                return;
            }

            startPoint = bossComp.GetPointTransform(startPointName);
            endPoint = bossComp.GetPointTransform(endPointName);

            if (startPoint == null || endPoint == null)
            {
                Debug.LogError(
                    $"LaserWallAttack: Missing boss points ({startPointName}, {endPointName})"
                );
                return;
            }

            boss.GetComponent<MonoBehaviour>()
                .StartCoroutine(SpawnLaserRows(boss));
        }

        private IEnumerator SpawnLaserRows(GameObject boss)
        {
            Vector3 start = startPoint.position;
            Vector3 end = endPoint.position;

            // Direction of progression
            Vector3 forwardDir = (end - start).normalized;
            forwardDir.y = 0f;

            // Sideways direction for wall spread
            Vector3 sideDir = Vector3.Cross(Vector3.up, forwardDir).normalized;

            float totalDistance = Vector3.Distance(start, end);
            float traveled = 0f;

            while (traveled <= totalDistance)
            {
                Vector3 rowCenter = start + forwardDir * traveled;

                SpawnRow(rowCenter, sideDir);

                traveled += rowSpacing;
                yield return new WaitForSeconds(rowInterval);
            }
        }

        private void SpawnRow(Vector3 center, Vector3 sideDir)
        {
            float halfWidth = (wallsPerRow - 1) * 0.5f;

            for (int i = 0; i < wallsPerRow; i++)
            {
                float offset = (i - halfWidth) * wallSpacing;
                Vector3 spawnPos = center + sideDir * offset;

                Instantiate(laserWallPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
