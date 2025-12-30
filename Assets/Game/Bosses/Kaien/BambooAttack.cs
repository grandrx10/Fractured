using System.Collections;
using UnityEngine;
using Game.Bosses;
using Characters;
using Game.Bosses.Projectiles;

[CreateAssetMenu(menuName = "BossAttacks/Kaien/BambooAttack")]
public class BambooAttack : BossAttack
{
    [Header("Spawn Settings")]
    public GameObject warningPrefab;
    public float warningDuration = 1f;
    public int warningRadius = 2;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;

    [Tooltip("Distance from target where projectile spawns")]
    public float spawnDistance = 12f;

    [Tooltip("Projectile speed toward the warning")]
    public float projectileSpeed = 20f;

    [Header("Attack Settings")]
    public float spawnInterval = 0.5f;

    [Header("Spread Settings")]
    public float spreadRadius = 0f;

    [Header("Angle Variation")]
    [Tooltip("Max angle (degrees) away from straight down")]
    public float maxAngleFromVertical = 20f;

    private GameObject bossGO;
    private float timer;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);
        bossGO = boss;
        timer = 0f;
    }

    public override void Tick(GameObject boss)
    {
        if (PlayerSingleton.Instance == null)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            bossGO.GetComponent<MonoBehaviour>()
                .StartCoroutine(SpawnWithWarning());
        }
    }

    private IEnumerator SpawnWithWarning()
    {
        Vector3 basePos = PlayerSingleton.Instance.GetPositionBelow();

        Vector2 rand = Random.insideUnitCircle * spreadRadius;
        Vector3 targetPos = basePos + new Vector3(rand.x, 0f, rand.y);

        // Warning
        if (warningPrefab != null)
        {
            Warning w = Instantiate(warningPrefab, targetPos, Quaternion.identity)
                .GetComponent<Warning>();

            if (w != null)
            {
                w.Initialize(
                    warningRadius,
                    spawnInterval,
                    Warning.WarningType.Grounded,
                    warningDuration
                );
            }
        }

        yield return new WaitForSeconds(warningDuration);

        if (projectilePrefab == null)
            yield break;

        /* ===============================
         * ANGLED SPAWN DIRECTION
         * =============================== */

        // Pick a random horizontal direction
        Vector2 horiz = Random.insideUnitCircle.normalized;

        // Tilt away from vertical by maxAngleFromVertical
        float angle = Random.Range(0f, maxAngleFromVertical) * Mathf.Deg2Rad;
        float y = -Mathf.Cos(angle);
        float horizScale = Mathf.Sin(angle);

        Vector3 attackDir = new Vector3(
            horiz.x * horizScale,
            y,
            horiz.y * horizScale
        ).normalized;

        // Spawn point
        Vector3 spawnPos = targetPos - attackDir * spawnDistance;

        // Velocity
        Vector3 velocity = (targetPos - spawnPos).normalized * projectileSpeed;

        // ROTATION: align local -Y to velocity
        Quaternion rotation = Quaternion.FromToRotation(Vector3.down, velocity.normalized);

        GameObject proj = Instantiate(
            projectilePrefab,
            spawnPos,
            rotation
        );

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb == null)
            rb = proj.AddComponent<Rigidbody>();

        rb.linearVelocity = velocity;
    }
}
