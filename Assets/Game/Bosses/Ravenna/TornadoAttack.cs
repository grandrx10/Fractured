using UnityEngine;
using Game.Bosses;

[CreateAssetMenu(menuName = "BossAttacks/Ravenna/TornadoAttack")]
public class TornadoAttack : BossAttack
{
    [Header("Tornado Settings")]
    public GameObject tornadoPrefab;
    public float spawnInterval = 1f;
    public int tornadoCount = 5;
    public float minRadius = 5f;
    public float yOffsetBelowBoss = 10f; // Fixed distance below the boss

    private float _timer;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);
        _timer = 0f;
    }

    public override void Tick(GameObject boss)
    {
        if (!isActive)
            return;

        _timer += Time.deltaTime;

        if (_timer >= spawnInterval)
        {
            _timer -= spawnInterval;
            SpawnTornadoRing(boss);
        }
    }

    public override void EndAttack(GameObject boss)
    {
        base.EndAttack(boss);
    }

    private void SpawnTornadoRing(GameObject boss)
    {
        Vector3 center = boss.transform.position;

        float angleStep = 360f / tornadoCount;
        float startAngle = Random.Range(0f, 360f);

        for (int i = 0; i < tornadoCount; i++)
        {
            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            // Random distance in XZ plane
            float radius = Random.Range(minRadius, minRadius * 2); // optional variation

            Vector3 offset = new Vector3(
                Mathf.Cos(rad) * radius, // X
                -yOffsetBelowBoss,       // Y fixed below boss
                Mathf.Sin(rad) * radius  // Z
            );

            Vector3 spawnPos = center + offset;

            Instantiate(
                tornadoPrefab,
                spawnPos,
                Quaternion.identity
            );
        }
    }
}
