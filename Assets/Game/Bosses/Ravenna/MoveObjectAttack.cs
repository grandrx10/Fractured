using UnityEngine;
using System.Collections;
using Game.Bosses;

[CreateAssetMenu(menuName = "BossAttacks/Common/MoveObjectAttack")]
public class MoveObjectAttack : BossAttack
{
    [Header("Movement Settings")]
    public string fromPointName;   // Name of the starting point on the boss
    public string toPointName;     // Name of the target point on the boss
    public float speed = 10f;      // Units per second

    public override void StartAttack(GameObject boss)
    {
        if (boss == null)
        {
            Debug.LogError("Boss is null in MoveObjectAttack!");
            return;
        }

        Boss bossComponent = boss.GetComponent<Boss>();
        if (bossComponent == null)
        {
            Debug.LogError("Boss component not found on the GameObject!");
            return;
        }

        // Get the transforms from named points
        Transform fromTransform = bossComponent.GetPointTransform(fromPointName);
        Transform toTransform = bossComponent.GetPointTransform(toPointName);

        if (fromTransform == null)
        {
            Debug.LogError($"From point '{fromPointName}' not found on boss {boss.name}");
            return;
        }

        if (toTransform == null)
        {
            Debug.LogError($"To point '{toPointName}' not found on boss {boss.name}");
            return;
        }

        // Start the movement coroutine
        boss.GetComponent<MonoBehaviour>().StartCoroutine(MoveObject(fromTransform, toTransform));
    }

    private IEnumerator MoveObject(Transform from, Transform to)
    {
        while (Vector3.Distance(from.position, to.position) > 0.01f)
        {
            from.position = Vector3.MoveTowards(from.position, to.position, speed * Time.deltaTime);
            yield return null;
        }
    }
}
