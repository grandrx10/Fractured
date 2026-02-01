using UnityEngine;
using Game.Bosses;
using System.Collections;
using System.Collections.Generic;
using Cards.Environments;

[CreateAssetMenu(menuName = "BossAttacks/Emperor/TimeStopAttack")]
public class TimeStopAttack : BossAttack
{
    public string fieldPointName;
    [Header("Audio")]
    public AudioClip timeStopSound;
    public float timeStopVolume = 1f;


    [Header("Scaling")]
    public float maxScale = 400f;
    public float growDuration = 3f;

    [Header("Breakaway Force")]
    public float outwardForce = 20f;
    public float upwardForce = 10f;

    [Header("Return")]
    public float returnDelay = 10f;
    public float playerUnfreezeDelay = 2.5f; // Delay after return before unfreezing player

    [Header("Breakaway Rotation")]
    public float minTorque = 5f;
    public float maxTorque = 20f;

    private Coroutine attackCoroutine;
    private Rigidbody playerRb;

    public override void StartAttack(GameObject bossObj)
    {
        base.StartAttack(bossObj);
        if (timeStopSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(
                timeStopSound,
                Vector3.zero,      // position irrelevant for 2D
                timeStopVolume,
                randomizePitch: false,
                spatialBlend: 0f   // 2D sound
            );
        }

        Boss boss = bossObj.GetComponent<Boss>();
        if (boss == null)
        {
            Debug.LogError("TimeStopAttack: Boss component not found.");
            return;
        }

        Transform field = boss.GetPointTransform(fieldPointName);
        if (field == null)
        {
            Debug.LogError($"TimeStopAttack: Named point '{fieldPointName}' not found.");
            return;
        }

        // Get player from OpenWorld
        Transform player = OpenWorldEnv.Current.PlayerTransform;
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
                playerRb.isKinematic = true;
        }

        attackCoroutine = boss.StartCoroutine(AttackRoutine(field));
    }

    public override void EndAttack(GameObject bossObj)
    {
        base.EndAttack(bossObj);
        attackCoroutine = null; // Let coroutine continue until completion
    }

    private IEnumerator AttackRoutine(Transform field)
    {
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * maxScale;
        field.localScale = startScale;

        HashSet<Breakaway> affected = new HashSet<Breakaway>();

        float elapsed = 0f;

        // Grow phase
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / growDuration);
            field.localScale = Vector3.Lerp(startScale, targetScale, t);

            CollectAndBreak(field, affected);

            yield return null;
        }

        field.localScale = targetScale;

        // Hold
        yield return new WaitForSeconds(returnDelay);

        // Return all breakaways
        foreach (var b in affected)
        {
            if (b != null)
                b.Return();
        }

        // Wait extra time before unfreezing player
        if (playerRb != null)
        {
            yield return new WaitForSeconds(playerUnfreezeDelay);
            playerRb.isKinematic = false;
        }

        field.localScale = startScale;
    }

    private void CollectAndBreak(Transform field, HashSet<Breakaway> set)
    {
        Collider trigger = field.GetComponent<Collider>();
        if (trigger == null || !trigger.isTrigger)
        {
            Debug.LogError("TimeStopAttack: Field must have a trigger collider.");
            return;
        }

        Collider[] hits = Physics.OverlapBox(
            trigger.bounds.center,
            trigger.bounds.extents,
            field.rotation
        );

        foreach (var hit in hits)
        {
            Breakaway b = hit.GetComponentInParent<Breakaway>();
            if (b == null || set.Contains(b))
                continue;

            b.Break();
            set.Add(b);

            Rigidbody rb = b.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 outwardDir = (b.transform.position - field.position).normalized;

                // Random direction that never points inward
                Vector3 randomDir = Random.onUnitSphere;
                if (Vector3.Dot(randomDir, outwardDir) < 0f)
                    randomDir = -randomDir;

                Vector3 finalDir = (outwardDir + randomDir).normalized;

                Vector3 force =
                    finalDir * outwardForce +
                    Vector3.up * upwardForce;

                rb.AddForce(force, ForceMode.Impulse);

                Vector3 randomTorque = Random.onUnitSphere * Random.Range(minTorque, maxTorque);
                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }
        }
    }
}
