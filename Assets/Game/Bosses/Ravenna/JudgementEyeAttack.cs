using UnityEngine;
using Game.Bosses;
using System.Collections;

[CreateAssetMenu(menuName = "BossAttacks/Ravenna/JudgementEyeAttack")]
public class JudgementEyeAttack : BossAttack
{
    [Header("Attack Settings")]
    public GameObject eyePrefab;          // Prefab to spawn
    public GameObject warningPrefab;      // Prefab for warning
    public string launchPointName;        // Named point on the boss
    public int shotCount = 5;             // Total shots to fire
    public float shotGap = 1f;            // Gap between shots
    public float warningDuration = 1f;    // How long the warning stays before each shot

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);

        Boss bossComponent = boss.GetComponent<Boss>();
        if (bossComponent == null)
        {
            Debug.LogError("Boss component not found on the GameObject!");
            return;
        }

        Transform launchPoint = bossComponent.GetPointTransform(launchPointName);
        if (launchPoint == null)
        {
            Debug.LogError($"Named point '{launchPointName}' not found on boss {boss.name}");
            return;
        }

        // Start coroutine on boss to handle full shot sequence
        MonoBehaviour mb = boss.GetComponent<MonoBehaviour>();
        if (mb != null)
        {
            mb.StartCoroutine(ShotSequence(launchPoint));
        }
        else
        {
            Debug.LogError("Boss has no MonoBehaviour component!");
        }
    }

    private IEnumerator ShotSequence(Transform launchPoint)
    {
        for (int i = 0; i < shotCount; i++)
        {
            // Spawn warning
            GameObject warning = null;
            if (warningPrefab != null)
            {
                warning = Object.Instantiate(warningPrefab, launchPoint.position, launchPoint.rotation, launchPoint);
            }

            // Wait for warningDuration
            if (warningDuration > 0f)
                yield return new WaitForSeconds(warningDuration);

            // Destroy warning
            if (warning != null)
                Object.Destroy(warning);

            // Spawn eye
            if (eyePrefab != null)
                Object.Instantiate(eyePrefab, launchPoint.position, launchPoint.rotation);

            // Wait for shotGap before next shot (except after last)
            if (i < shotCount - 1)
                yield return new WaitForSeconds(shotGap);
        }
    }
}
