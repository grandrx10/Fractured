using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "BossAttacks/Rock/StompAttack")]
public class StompAttack : BossAttack
{
    public float cooldown = 1f;

    [Header("Jump Settings")]
    public float jumpDuration = 1f;           // constant duration for all jumps
    public float minPeakHeight = 2f;          // height when jumping max distance
    public float maxPeakHeight = 8f;          // height when jumping close range
    public float maxJumpDistance = 10f;       // maximum distance boss can jump
    public float targetRadius = 2f;           // random offset around player

    [Header("Warning Settings")]
    public GameObject warningPrefab;
    public int warningRadius;

    private float lastAttackTime;
    private bool isJumping = false;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);
        lastAttackTime = -cooldown;
    }

    public override void Tick(GameObject boss)
    {
        if (!isActive || isJumping) return;
        if (PlayerSingleton.Instance == null) return;

        if (Time.time - lastAttackTime >= cooldown)
        {
            Vector3 playerPos = PlayerSingleton.Instance.transform.position;

            // Add randomness to target
            Vector2 randomOffset = Random.insideUnitCircle * targetRadius;
            Vector3 targetPos = playerPos + new Vector3(randomOffset.x, 0f, randomOffset.y);

            // Limit maximum jump distance
            Vector3 startPos = boss.transform.position;
            Vector3 toTarget = targetPos - startPos;
            if (toTarget.magnitude > maxJumpDistance)
            {
                toTarget = toTarget.normalized * maxJumpDistance;
                targetPos = startPos + toTarget;
            }

            // Spawn warning prefab
            if (warningPrefab != null)
            {
                Warning w = Instantiate(warningPrefab, targetPos, Quaternion.identity).GetComponent<Warning>();
                w.Initialize(warningRadius, jumpDuration, Warning.WarningType.Grounded);
            }

            // Smooth rotation
            NpcCommands npc = boss.GetComponent<NpcCommands>();
            if (npc != null)
                npc.RotateOnceTowards(PlayerSingleton.Instance.transform);

            // Start jump
            boss.GetComponent<MonoBehaviour>().StartCoroutine(JumpParabola(boss, targetPos));
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator JumpParabola(GameObject boss, Vector3 targetPos)
    {
        isJumping = true;

        Vector3 startPos = boss.transform.position;
        targetPos.y = startPos.y;

        float distance = Vector3.Distance(startPos, targetPos);
        
        // Calculate peak height based on distance (closer = higher jump)
        float normalizedDistance = Mathf.Clamp01(distance / maxJumpDistance);
        float peakHeight = Mathf.Lerp(maxPeakHeight, minPeakHeight, normalizedDistance);

        // Gravity & initial vertical speed for calculated peakHeight
        float gravity = 2f * peakHeight / Mathf.Pow(jumpDuration / 2f, 2);
        float initialVerticalSpeed = gravity * (jumpDuration / 2f);

        float time = 0f;
        while (time < jumpDuration)
        {
            float t = time;
            Vector3 horiz = Vector3.Lerp(startPos, targetPos, t / jumpDuration);
            float y = startPos.y + initialVerticalSpeed * t - 0.5f * gravity * t * t;

            boss.transform.position = new Vector3(horiz.x, y, horiz.z);

            time += Time.deltaTime;
            yield return null;
        }

        boss.transform.position = targetPos;
        isJumping = false;
    }
}