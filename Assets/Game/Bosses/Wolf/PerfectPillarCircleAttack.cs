using System.Collections;
using UnityEngine;
using Characters;

namespace Game.Bosses.Wolf
{
    [CreateAssetMenu(menuName = "BossAttacks/Wolf/PerfectPillarCircleAttack")]
    public class PerfectPillarCircleAttack : BossAttack
    {
        [Header("Pillar Settings")]
        public GameObject pillarPrefab;
        public int pillarCount = 12;
        public float circleRadius = 10f;

        [Header("Rise Settings")]
        public float riseSpeed = 15f;
        public float riseHeight = 12f;

        private MonoBehaviour coroutineRunner;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            coroutineRunner = boss.GetComponent<MonoBehaviour>();
            if (coroutineRunner == null)
            {
                Debug.LogError("PerfectPillarCircleAttack: Boss has no MonoBehaviour");
                return;
            }

            coroutineRunner.StartCoroutine(SpawnCircle());
        }

        private IEnumerator SpawnCircle()
        {
            if (pillarPrefab == null || PlayerSingleton.Instance == null)
                yield break;

            Vector3 playerPos = PlayerSingleton.Instance.GetPositionBelow();
            float angleStep = 360f / pillarCount;

            for (int i = 0; i < pillarCount; i++)
            {
                float angleRad = angleStep * i * Mathf.Deg2Rad;

                Vector3 offset = new Vector3(
                    Mathf.Cos(angleRad),
                    0f,
                    Mathf.Sin(angleRad)
                ) * circleRadius;

                Vector3 spawnPos = playerPos + offset;

                // 🔹 Make pillar face outward from the center
                Vector3 radialDir = offset.normalized;
                Quaternion rotation = Quaternion.LookRotation(radialDir, Vector3.up);

                GameObject pillar = Instantiate(
                    pillarPrefab,
                    spawnPos,
                    rotation
                );

                Collider col = pillar.GetComponent<Collider>();
                if (col != null)
                    col.enabled = false;

                coroutineRunner.StartCoroutine(
                    RisePillar(pillar.transform, col)
                );
            }

            yield return null;
        }


        private IEnumerator RisePillar(Transform pillar, Collider pillarCollider)
        {
            Vector3 startPos = pillar.position;
            Vector3 targetPos = startPos + Vector3.up * riseHeight;

            float moved = 0f;

            while (pillar != null && moved < riseHeight)
            {
                float step = riseSpeed * Time.deltaTime;
                pillar.position += Vector3.up * step;
                moved += step;
                yield return null;
            }

            if (pillar != null)
            {
                pillar.position = targetPos;
                if (pillarCollider != null)
                    pillarCollider.enabled = true;
            }
        }
    }
}
