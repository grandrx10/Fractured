using UnityEngine;
using Game.Bosses;
using Cards.Environments;
using System.Collections;

[CreateAssetMenu(menuName = "BossAttacks/Emperor/TeleportSummonAttack")]
public class TeleportSummonAttack : BossAttack
{
    [Header("Teleport")]
    public float teleportRadius = 20f;
    public float minTeleportDistance = 10f;
    public string returnPointName; // named point on boss

    [Header("Summon")]
    public GameObject bladePrefab;
    public float spawnHeight = 40f;
    public float spawnRadiusXZ = 10f;

    [Header("Timing")]
    public float interval = 1f;

    private Coroutine attackCoroutine;
    private Transform returnPoint;

    public override void StartAttack(GameObject bossObj)
    {
        base.StartAttack(bossObj);

        Boss boss = bossObj.GetComponent<Boss>();
        if (boss == null)
        {
            Debug.LogError("TeleportSummonAttack: Boss component missing.");
            return;
        }

        returnPoint = boss.GetPointTransform(returnPointName);
        if (returnPoint == null)
        {
            Debug.LogError($"TeleportSummonAttack: Return point '{returnPointName}' not found.");
            return;
        }

        attackCoroutine = boss.StartCoroutine(AttackLoop(bossObj));
    }

    public override void EndAttack(GameObject bossObj)
    {
        base.EndAttack(bossObj);

        if (attackCoroutine != null)
        {
            bossObj.GetComponent<Boss>().StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        // Teleport boss back to start
        if (returnPoint != null)
        {
            bossObj.transform.position = returnPoint.position;
            bossObj.transform.rotation = returnPoint.rotation;
        }

        // Trigger all delayed heaven blades
        DelayedTriggerHeavenBlade[] blades =
            GameObject.FindObjectsOfType<DelayedTriggerHeavenBlade>();

        foreach (var blade in blades)
        {
            blade.Activate();
        }
    }

    private IEnumerator AttackLoop(GameObject bossObj)
    {
        Transform player = OpenWorldEnv.Current.PlayerTransform;

        while (isActive)
        {
            TeleportBoss(bossObj, player);
            SpawnBladeAboveBoss(bossObj.transform);

            yield return new WaitForSeconds(interval);
        }
    }

    private void TeleportBoss(GameObject bossObj, Transform player)
    {
        Vector3 randomDir = Random.insideUnitSphere;
        randomDir.y = 0f;
        randomDir.Normalize();

        float distance = Random.Range(minTeleportDistance, teleportRadius);
        Vector3 targetPos = player.position + randomDir * distance;

        bossObj.transform.position = targetPos;
    }

    private void SpawnBladeAboveBoss(Transform bossTransform)
    {
        Vector2 randomXZ = Random.insideUnitCircle * spawnRadiusXZ;
        Vector3 spawnPos = bossTransform.position +
                           new Vector3(randomXZ.x, spawnHeight, randomXZ.y);

        float randomYRotation = Random.Range(0f, 360f);
        Quaternion rotation = Quaternion.Euler(0f, randomYRotation, 0f);

        GameObject.Instantiate(bladePrefab, spawnPos, rotation);
    }
}
