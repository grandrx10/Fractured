using Cards.Environments;
using Characters;
using UnityEngine;

namespace Game.Bosses.Rock
{
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
        private GameObject currentRock;
        private Transform spawnPoint;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);
            lastAttackTime = -cooldown;

            npc = boss.GetComponent<NpcCommands>();
            if (npc != null)
                npc.SetLookingAt(OpenWorldEnv.Current.PlayerTransform);

            // Get the spawn transform
            spawnPoint = boss.GetComponent<Boss>().GetPointTransform(spawnPointName);

            isWindingUp = true;
            windupTimer = 0f;

            if (spawnPoint != null && rockPrefab != null)
            {
                // Spawn rock at small scale, make kinematic, parent to spawn point
                currentRock = GameObject.Instantiate(rockPrefab, spawnPoint.position, Quaternion.identity);
                currentRock.transform.localScale = Vector3.one * 0.2f;

                Rigidbody rb = currentRock.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.isKinematic = true;

                currentRock.transform.SetParent(spawnPoint);
            }
        }

        public override void Tick(GameObject boss)
        {
            if (!isActive) return;

            Transform player = OpenWorldEnv.Current.PlayerTransform;

            if (isWindingUp && currentRock != null)
            {
                windupTimer += Time.deltaTime;

                // Interpolation factor
                float t = Mathf.Clamp01(windupTimer / windupDuration);

                // Get parent scale
                Vector3 parentScale = currentRock.transform.parent.lossyScale;

                // Desired world scale from 0.2 → 1
                float worldScale = Mathf.Lerp(0.2f, 1f, t);

                // Convert to local scale so world scale = desired
                currentRock.transform.localScale = new Vector3(
                    worldScale / parentScale.x,
                    worldScale / parentScale.y,
                    worldScale / parentScale.z
                );

                // Launch at end of windup
                if (windupTimer >= windupDuration)
                {
                    LaunchRock(currentRock, player.position);
                    lastAttackTime = Time.time;
                    isWindingUp = false;
                    currentRock = null;
                }
            }

            else if (Time.time - lastAttackTime >= cooldown)
            {
                // Prepare next rock
                isWindingUp = true;
                windupTimer = 0f;

                if (spawnPoint != null && rockPrefab != null)
                {
                    currentRock = GameObject.Instantiate(rockPrefab, spawnPoint.position, Quaternion.identity);

                    // Make kinematic
                    Rigidbody rb = currentRock.GetComponent<Rigidbody>();
                    if (rb != null)
                        rb.isKinematic = true;

                    // Parent to spawn point
                    currentRock.transform.SetParent(spawnPoint);

                    // Set initial local scale so world scale = 0.2
                    Vector3 parentScale = spawnPoint.lossyScale;
                    currentRock.transform.localScale = new Vector3(
                        0.2f / parentScale.x,
                        0.2f / parentScale.y,
                        0.2f / parentScale.z
                    );
                }
            }
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);
            if (npc != null)
                npc.SetLookingAt(null);
        }

        private void LaunchRock(GameObject rock, Vector3 target)
        {
            if (rock == null) return;

            Rigidbody rb = rock.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning("Rock prefab must have a Rigidbody!");
                return;
            }

            // Unparent and enable physics
            rock.transform.SetParent(null);
            rb.isKinematic = false;

            Vector3 direction = (target - rock.transform.position).normalized;
            rb.AddForce(direction * launchForce, ForceMode.Impulse);
        }
    }
}
