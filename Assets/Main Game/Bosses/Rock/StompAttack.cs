using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "BossAttacks/Rock/StompAttack")]
public class StompAttack : BossAttack
{
    public float cooldown = 1f;
    public float launchForce = 20f;
    public float upwardBoost = 1.5f;
    public float rotationDuration = 0.5f; // Time to rotate toward player

    private float lastAttackTime;

    public override void StartAttack(GameObject boss)
    {
        base.StartAttack(boss);
        lastAttackTime = -cooldown;
    }

    public override void Tick(GameObject boss)
    {
        if (!isActive) return;

        Rigidbody rb = boss.GetComponent<Rigidbody>();
        if (rb == null) return;

        if (PlayerSingleton.Instance == null) return;
        Transform player = PlayerSingleton.Instance.transform;

        // Launch toward player if cooldown passed
        if (Time.time - lastAttackTime >= cooldown)
        {
            LaunchAtPlayer(rb, player);
            NpcCommands npc = boss.GetComponent<NpcCommands>();
            if (npc != null && PlayerSingleton.Instance != null)
            {
                npc.RotateOnceTowards(PlayerSingleton.Instance.transform); // single rotation
            }


            lastAttackTime = Time.time;
        }
    }

    private void LaunchAtPlayer(Rigidbody rb, Transform player)
    {
        Vector3 direction = (player.position - rb.position).normalized;
        direction.y = upwardBoost;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction.normalized * launchForce, ForceMode.Impulse);
    }
}
