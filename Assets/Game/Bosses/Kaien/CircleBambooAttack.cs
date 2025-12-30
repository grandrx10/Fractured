using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Bosses;
using Characters;
using Game.Bosses.Projectiles; // Required for Warning

[CreateAssetMenu(menuName = "BossAttacks/Kaien/CircleBambooAttack")]
public class CircleBambooAttack : BossAttack
{
    [Header("Spawn Settings")]
    public GameObject warningPrefab;
    public float warningDuration = 1f;
    public float spawnInterval = 2f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Vector3 spawnOffset = new Vector3(0f, 10f, 0f);
    public Vector3 downwardVelocity = new Vector3(0f, -20f, 0f);

    [Header("Circle Settings")]
    public int totalBamboo = 12;
    public float radius = 10f;
    public int exitCount = 3; // consecutive gaps

    private bool isRunning = false;
    private GameObject bossGO;
    private float timer;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);
        bossGO = boss;
        isRunning = true;
        timer = 0f;

        // Immediately start the first circle
        bossGO.GetComponent<MonoBehaviour>().StartCoroutine(SpawnCircleBamboo());
    }

    public override void EndAttack(GameObject boss)
    {
        base.EndAttack(boss);
        isRunning = false;
    }

    public override void Tick(GameObject boss)
    {
        if (!isRunning || PlayerSingleton.Instance == null)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            bossGO.GetComponent<MonoBehaviour>().StartCoroutine(SpawnCircleBamboo());
        }
    }

    private IEnumerator SpawnCircleBamboo()
    {
        Vector3 playerPos = PlayerSingleton.Instance.GetPositionBelow();

        int exitStart = Random.Range(0, totalBamboo);

        List<Vector3> spawnPositions = new List<Vector3>();

        // Compute all spawn positions first (skip exits)
        for (int i = 0; i < totalBamboo; i++)
        {
            int distanceFromExit = (i - exitStart + totalBamboo) % totalBamboo;
            if (distanceFromExit < exitCount)
                continue;

            float angle = i * Mathf.PI * 2f / totalBamboo;
            Vector3 spawnPos = playerPos + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            spawnPositions.Add(spawnPos);

            // Spawn warning
            if (warningPrefab != null)
            {
                Warning w = Instantiate(warningPrefab, spawnPos, Quaternion.identity).GetComponent<Warning>();
                if (w != null)
                {
                    w.Initialize(1f, spawnInterval, Warning.WarningType.Grounded, warningDuration);
                }
            }
        }

        // Wait for all warnings to finish
        yield return new WaitForSeconds(warningDuration);

        // Spawn all bamboo projectiles above their warnings at once
        foreach (Vector3 pos in spawnPositions)
        {
            if (projectilePrefab != null)
            {
                Vector3 bambooSpawnPos = pos + spawnOffset;
                GameObject proj = Instantiate(projectilePrefab, bambooSpawnPos, Quaternion.identity);

                Rigidbody rb = proj.GetComponent<Rigidbody>();
                if (rb == null)
                    rb = proj.AddComponent<Rigidbody>();

                rb.linearVelocity = downwardVelocity;
            }
        }
    }
}
