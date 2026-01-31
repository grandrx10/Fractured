using UnityEngine;
using Game.Bosses;
using System.Collections;

[CreateAssetMenu(menuName = "BossAttacks/Emperor/CrossSwordAttack")]
public class CrossSwordAttack : BossAttack
{
    public GameObject crossSwordPrefab;

    public string startPointName;
    public string endPointName;

    public float spawnInterval = 3f;

    private Coroutine spawnCoroutine;

    public override void StartAttack(GameObject bossObj)
    {
        base.StartAttack(bossObj);

        Boss boss = bossObj.GetComponent<Boss>();
        if (boss == null)
        {
            Debug.LogError("CrossSwordAttack: Boss component not found.");
            return;
        }

        spawnCoroutine = boss.StartCoroutine(SpawnLoop(boss));
    }

    public override void EndAttack(GameObject bossObj)
    {
        base.EndAttack(bossObj);

        if (spawnCoroutine != null)
        {
            bossObj.GetComponent<Boss>().StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop(Boss boss)
    {
        Transform start = boss.GetPointTransform(startPointName);
        Transform end = boss.GetPointTransform(endPointName);

        if (start == null || end == null)
        {
            Debug.LogError("CrossSwordAttack: Start or End point missing on Boss.");
            yield break;
        }

        while (isActive)
        {
            GameObject sword = Object.Instantiate(
                crossSwordPrefab,
                start.position,
                Quaternion.identity
            );

            CrossSword swordLogic = sword.GetComponent<CrossSword>();
            if (swordLogic != null)
            {
                swordLogic.Initialize(start.position, end.position);
            }

            // IMPORTANT: countdown starts immediately, not after sword finishes
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
