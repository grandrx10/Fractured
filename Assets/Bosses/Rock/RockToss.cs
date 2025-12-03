using UnityEngine;

[CreateAssetMenu(menuName = "BossAttacks/Rock/RockToss")]
public class RockToss : BossAttack
{
    public GameObject rockPrefab;       // Rock prefab
    public float launchForce = 15f;     // Impulse force
    public float cooldown = 1f;         // Time between tosses
    public float windupDuration = 0.5f; // Pause before tossing
    public string spawnPointName;       // Which boss point to spawn from

    private float lastAttackTime;
    private float windupTimer = 0f;
    private bool isWindingUp = false;
    private NpcCommands npc;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);
        lastAttackTime = -cooldown;

        npc = boss.GetComponent<NpcCommands>();
        if (npc != null && PlayerSingleton.Instance != null)
            npc.SetLookingAt(PlayerSingleton.Instance.transform);

        isWindingUp = true;
        windupTimer = 0f;
    }

    public override void Tick(GameObject boss)
    {
        if (!isActive) return;
        if (PlayerSingleton.Instance == null) return;

        Transform player = PlayerSingleton.Instance.transform;

        Vector3 spawnPos = boss.GetComponent<Boss>().GetPointPosition(spawnPointName);

        if (isWindingUp)
        {
            windupTimer += Time.deltaTime;
            if (windupTimer >= windupDuration)
            {
                TossRock(spawnPos, player.position);
                lastAttackTime = Time.time;
                isWindingUp = false;
            }
        }
        else if (Time.time - lastAttackTime >= cooldown)
        {
            isWindingUp = true;
            windupTimer = 0f;
        }
    }

    public override void EndAttack(GameObject boss)
    {
        base.EndAttack(boss);
        if (npc != null)
            npc.SetLookingAt(null);
    }

    private void TossRock(Vector3 origin, Vector3 target)
    {
        if (rockPrefab == null) return;

        GameObject rock = GameObject.Instantiate(rockPrefab, origin, Quaternion.identity);
        Rigidbody rb = rock.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("Rock prefab must have a Rigidbody!");
            return;
        }

        Vector3 direction = (target - origin).normalized;
        rb.AddForce(direction * launchForce, ForceMode.Impulse);
    }
}
