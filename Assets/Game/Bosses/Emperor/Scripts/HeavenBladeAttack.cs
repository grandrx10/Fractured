using UnityEngine;
using Game.Bosses;
using Cards.Environments;
using System.Collections;

[CreateAssetMenu(menuName = "BossAttacks/Emperor/HeavenBladeAttack")]
public class HeavenBladeAttack : BossAttack
{
    public GameObject bladePrefab;        // Prefab of the Heaven Blade
    public float spawnHeight = 40f;       // Height above player
    public float spawnRadiusXZ = 30f;     // XZ radius around player
    public float spawnInterval = 0.5f;    // Time between each blade spawn

    private Coroutine spawnCoroutine;
    private float modifiedSpawnInterval;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);

        // Start continuous spawning
        MonoBehaviour mono = boss.GetComponent<MonoBehaviour>();
        if (mono != null)
        {
            spawnCoroutine = mono.StartCoroutine(SpawnBladesContinuously());
        }
        else
        {
            Debug.LogError("Boss must have a MonoBehaviour to start coroutines!");
        }

        modifiedSpawnInterval = spawnInterval;
    }

    public override void EndAttack(GameObject boss)
    {
        base.EndAttack(boss);

        // Stop spawning when attack ends
        if (spawnCoroutine != null)
        {
            MonoBehaviour mono = boss.GetComponent<MonoBehaviour>();
            mono.StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnBladesContinuously()
    {
        var player = OpenWorldEnv.Current.PlayerTransform;

        while (isActive) // keep spawning while attack is active
        {
            // Random position around player
            Vector2 randomXZ = Random.insideUnitCircle * spawnRadiusXZ;
            Vector3 spawnPos = player.position + new Vector3(randomXZ.x, spawnHeight, randomXZ.y);

            // Random rotation on XZ plane
            float randomYRotation = Random.Range(0f, 360f);
            Quaternion spawnRot = Quaternion.Euler(0, randomYRotation, 0);

            // Spawn blade
            GameObject blade = GameObject.Instantiate(bladePrefab, spawnPos, spawnRot);

            // Wait for next spawn
            yield return new WaitForSeconds(modifiedSpawnInterval);
            modifiedSpawnInterval = modifiedSpawnInterval * 0.9f;
        }
    }
}
