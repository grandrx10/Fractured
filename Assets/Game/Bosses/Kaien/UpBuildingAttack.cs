using UnityEngine;
using Game.Bosses;

[CreateAssetMenu(menuName = "BossAttacks/Gus/UpperBuildingAttack")]
public class UpperBuildingAttack : BossAttack
{
    [Header("Building Selection")]
    public GameObject[] buildingPrefabs;

    [Header("Spawn Points (Boss Named Points)")]
    public string[] spawnPointNames;

    [Header("Throw")]
    public float throwForce = 18f;
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

    private float spawnTimer;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);
        spawnTimer = 0f;
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

    private void SpawnBuilding(GameObject boss)
    {
        if (buildingPrefabs == null || buildingPrefabs.Length == 0)
            return;

        if (spawnPointNames == null || spawnPointNames.Length == 0)
            return;

        Boss bossComponent = boss.GetComponent<Boss>();
        if (bossComponent == null)
            return;

        string pointName = spawnPointNames[Random.Range(0, spawnPointNames.Length)];
        Transform spawnPoint = bossComponent.GetPointTransform(pointName);
        if (spawnPoint == null)
            return;

        GameObject prefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];

        GameObject building = Instantiate(prefab, spawnPoint.position, Random.rotation);

        Rigidbody rb = building.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning($"{prefab.name} has no Rigidbody.");
            return;
        }

        // Start small
        building.transform.localScale = Vector3.zero;

        // Throw straight up
        rb.AddForce(Vector3.up * throwForce, forceMode);

        // Spin
        Vector3 spin = new Vector3(
            Random.Range(minSpin.x, maxSpin.x),
            Random.Range(minSpin.y, maxSpin.y),
            Random.Range(minSpin.z, maxSpin.z)
        );
        rb.AddTorque(spin, forceMode);

        // Scale growth
        float targetScale = Random.Range(minMaxScale, maxMaxScale);
        boss.GetComponent<MonoBehaviour>()
            .StartCoroutine(GrowScale(building.transform, targetScale));
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
}
