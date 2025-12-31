using UnityEngine;
using Game.Bosses;

[CreateAssetMenu(menuName = "BossAttacks/Kaien/BuildingAttack")]
public class BuildingAttack : BossAttack
{
    [Header("Building Selection")]
    public GameObject[] buildingPrefabs;

    [Header("Spawn Center")]
    [Tooltip("NamedPoint used as the center of the spawn circle. If empty or not found, uses the boss.")]
    public string spawnCenterPointName;

    [Header("Spawn")]
    public float minSpawnRadius = 6f;
    public float maxSpawnRadius = 10f;
    public float spawnHeightOffset = 4f;

    [Header("Throw")]
    public float throwForce = 18f;
    public float coneAngle = 15f;
    public ForceMode forceMode = ForceMode.Impulse;

    [Header("Spin")]
    public Vector3 minSpin = new Vector3(-4f, -6f, -4f);
    public Vector3 maxSpin = new Vector3(4f, 6f, 4f);

    [Header("Interval")]
    public float spawnInterval = 0.35f;

    [Header("Scale Growth")]
    public float minMaxScale = 5f;
    public float maxMaxScale = 7f;
    public float scaleGrowTime = 0.25f;

    [Header("Lifetime")]
    public float buildingLifetime = 10f;

    [Header("Manual Skip Trigger")]
    public string riseTriggerPointName;

    private float spawnTimer;
    private Boss bossRef;
    private RiseTrigger boundTrigger;
    private Transform spawnCenter;   // <-- resolved spawn center

    public override void StartAttack(GameObject bossGO)
    {
        base.StartAttack(bossGO);
        spawnTimer = 0f;

        bossRef = bossGO.GetComponent<Boss>();
        if (bossRef == null)
            return;

        // Resolve spawn center
        spawnCenter = ResolveSpawnCenter(bossRef);

        // Bind manual skip trigger
        Transform triggerTransform = bossRef.GetPointTransform(riseTriggerPointName);
        if (triggerTransform == null)
        {
            Debug.LogWarning($"BuildingAttack: NamedPoint '{riseTriggerPointName}' not found.");
            return;
        }

        boundTrigger = triggerTransform.GetComponent<RiseTrigger>();
        if (boundTrigger == null)
        {
            Debug.LogWarning("BuildingAttack: RiseTrigger not found on NamedPoint.");
            return;
        }

        boundTrigger.OnTriggered += OnRiseTriggered;
    }

    public override void Tick(GameObject boss)
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnBuilding(boss);
        }
    }

    private void OnRiseTriggered()
    {
        if (bossRef != null)
            bossRef.EndCurrentAttack();
    }

    public override void EndAttack(GameObject boss)
    {
        if (boundTrigger != null)
            boundTrigger.OnTriggered -= OnRiseTriggered;

        boundTrigger = null;
        bossRef = null;
        spawnCenter = null;

        base.EndAttack(boss);
    }

    private Transform ResolveSpawnCenter(Boss boss)
    {
        if (!string.IsNullOrEmpty(spawnCenterPointName))
        {
            Transform t = boss.GetPointTransform(spawnCenterPointName);
            if (t != null)
                return t;

            Debug.LogWarning(
                $"BuildingAttack: Spawn center '{spawnCenterPointName}' not found. Falling back to boss."
            );
        }

        return boss.transform;
    }

    private void SpawnBuilding(GameObject boss)
    {
        if (buildingPrefabs == null || buildingPrefabs.Length == 0)
            return;

        GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];

        Vector2 circle = Random.insideUnitCircle.normalized *
                         Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector3 centerPos = spawnCenter != null
            ? spawnCenter.position
            : boss.transform.position;

        Vector3 spawnPos = centerPos +
                           new Vector3(circle.x, spawnHeightOffset, circle.y);

        GameObject building = Instantiate(prefab, spawnPos, Random.rotation);
        Destroy(building, buildingLifetime);

        Rigidbody rb = building.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"{prefab.name} has no Rigidbody.");
            return;
        }

        building.transform.localScale = Vector3.zero;

        Vector3 dir = GetDirectionTowardsPlayerWithCone(spawnPos, coneAngle);
        rb.linearVelocity = dir * throwForce;

        Vector3 spin = new Vector3(
            Random.Range(minSpin.x, maxSpin.x),
            Random.Range(minSpin.y, maxSpin.y),
            Random.Range(minSpin.z, maxSpin.z)
        );
        rb.AddTorque(spin, forceMode);

        float targetScale = Random.Range(minMaxScale, maxMaxScale);
        boss.GetComponent<MonoBehaviour>()
            .StartCoroutine(GrowScale(building.transform, targetScale));
    }

    private Vector3 GetDirectionTowardsPlayerWithCone(Vector3 spawnPos, float angle)
    {
        if (Characters.PlayerSingleton.Instance == null)
            return Vector3.down;

        Transform playerTransform = Characters.PlayerSingleton.Instance.transform;
        Vector3 baseDir = (playerTransform.position - spawnPos).normalized;
        return RandomConeDirection(baseDir, angle);
    }

    private System.Collections.IEnumerator GrowScale(Transform t, float targetScale)
    {
        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.one * targetScale;

        float time = 0f;
        while (time < scaleGrowTime)
        {
            time += Time.deltaTime;
            float t01 = time / scaleGrowTime;
            t.localScale = Vector3.Lerp(start, end, t01);
            yield return null;
        }

        t.localScale = end;
    }

    private Vector3 RandomConeDirection(Vector3 direction, float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        Vector2 randomCircle = Random.insideUnitCircle * Mathf.Tan(angleRad);
        Vector3 localDir = new Vector3(randomCircle.x, randomCircle.y, 1f).normalized;
        return Quaternion.LookRotation(direction) * localDir;
    }
}
