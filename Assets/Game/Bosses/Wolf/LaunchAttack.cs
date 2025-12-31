using System.Collections;
using Characters;
using UnityEngine;

namespace Game.Bosses.Wolf
{
    [CreateAssetMenu(menuName = "BossAttacks/Wolf/LaunchPillarAttack")]
    public class LaunchAttack : BossAttack
    {
        [Header("Spawn Settings")]
        public string spawnPointName;
        public GameObject pillarPrefab;

        [Header("Attack Settings")]
        public float attackInterval = 0.5f;      // Time between spawns
        public float pillarTravelTime = 0.5f;    // Time to reach random radius before shooting
        public float delayBeforeShoot = 0.5f;    // Delay after positioning before launching

        [Header("Movement Settings (Boss)")]
        public string movePointName;
        public float moveToPointSpeed = 5f;
        public float arriveThreshold = 0.1f;

        [Header("Pillar Radius Settings")]
        public float minRadius = 3f;
        public float maxRadius = 6f;
        public float minHeightAboveBoss = 2f;
        public float maxHeightAboveBoss = 5f;

        [Header("Pillar Shoot Settings")]
        public float shootSpeed = 20f;

        private Transform spawnPoint;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            Boss bossComp = boss.GetComponent<Boss>();
            if (bossComp == null)
            {
                Debug.LogError("LaunchAttack: Boss component missing");
                return;
            }

            spawnPoint = bossComp.GetPointTransform(spawnPointName);
            if (spawnPoint == null)
            {
                Debug.LogError($"LaunchAttack: Spawn point not found: {spawnPointName}");
                return;
            }

            Transform movePoint = bossComp.GetPointTransform(movePointName);
            if (movePoint == null)
            {
                Debug.LogError($"LaunchAttack: Move point not found: {movePointName}");
                return;
            }

            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null && PlayerSingleton.Instance != null)
                npcCommands.SetLookingAt(PlayerSingleton.Instance.transform);

            MonoBehaviour runner = boss.GetComponent<MonoBehaviour>();
            if (runner != null)
                runner.StartCoroutine(MoveThenAttack(boss.transform, movePoint, runner));
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);

            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null)
                npcCommands.SetLookingAt(null);
        }

        private IEnumerator MoveThenAttack(Transform bossTransform, Transform target, MonoBehaviour runner)
{
    while (Vector3.Distance(bossTransform.position, target.position) > arriveThreshold)
    {
        bossTransform.position = Vector3.MoveTowards(
            bossTransform.position,
            target.position,
            moveToPointSpeed * Time.deltaTime
        );
        yield return null;
    }

    bossTransform.position = target.position;

    // Start continuous pillar spawning, pass bossTransform
    yield return runner.StartCoroutine(FireLoop(runner, bossTransform));
}

private IEnumerator FireLoop(MonoBehaviour runner, Transform bossTransform)
{
    while (isActive && PlayerSingleton.Instance != null)
    {
        SpawnPillar(runner, bossTransform); // pass bossTransform
        yield return new WaitForSeconds(attackInterval);
    }
}

private void SpawnPillar(MonoBehaviour runner, Transform bossTransform)
{
    if (spawnPoint == null || PlayerSingleton.Instance == null || pillarPrefab == null)
        return;

    // Instantiate pillar at the spawn point
    GameObject pillar = Object.Instantiate(pillarPrefab, spawnPoint.position, Quaternion.identity);
    Rigidbody rb = pillar.GetComponentInChildren<Rigidbody>();

    if (rb == null)
    {
        Debug.LogError("Pillar prefab or its children must have a Rigidbody!");
        return;
    }

    // Preparation phase: kinematic
    rb.isKinematic = true;

    // Random vertical offset above the boss
    float heightOffset = Random.Range(minHeightAboveBoss, maxHeightAboveBoss);

    // Random direction in back hemisphere
    Vector3 backward = -bossTransform.forward; // exact backward
    float angleRange = 90f; // 90 degrees on each side of backward = back quarter-sphere
    Quaternion randomRotation = Quaternion.Euler(
        Random.Range(-angleRange / 2f, angleRange / 2f),
        Random.Range(-angleRange / 2f, angleRange / 2f),
        0
    );
    Vector3 randomDir = randomRotation * backward;

    // Random distance within min/max radius
    float distance = Random.Range(minRadius, maxRadius);
    Vector3 targetPos = bossTransform.position + randomDir.normalized * distance + Vector3.up * heightOffset;

    // Rotate pillar so "up" points to player from the target hover position
    Vector3 directionToPlayer = (PlayerSingleton.Instance.transform.position - targetPos).normalized;
    pillar.transform.rotation = Quaternion.FromToRotation(Vector3.up, directionToPlayer);

    // Move pillar to targetPos over pillarTravelTime
    runner.StartCoroutine(MovePillarToPosition(rb, targetPos, pillarTravelTime, runner));
}




        private IEnumerator MovePillarToPosition(Rigidbody rb, Vector3 targetPos, float duration, MonoBehaviour runner)
        {
            if (rb == null)
                yield break;

            Vector3 startPos = rb.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                rb.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            rb.position = targetPos;

            // After delay, launch toward player
            runner.StartCoroutine(ShootAfterDelay(rb, delayBeforeShoot, PlayerSingleton.Instance.transform.position));
        }

        private IEnumerator ShootAfterDelay(Rigidbody rb, float delay, Vector3 playerPos)
        {
            yield return new WaitForSeconds(delay);

            if (rb == null)
                yield break;

            rb.isKinematic = false;

            Vector3 direction = (playerPos - rb.position).normalized;
            rb.linearVelocity = direction * shootSpeed;
        }
    }
}
