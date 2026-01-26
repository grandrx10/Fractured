using UnityEngine;
using Cards.Environments;

namespace Game.Minions
{
    public class CrowMinionAttack : MinionAttack
    {
        [Header("Spawn Settings")]
        public GameObject prefabToSpawn;
        public Transform spawnPoint;
        public float interval = 1f; // seconds between spawns

        [Header("Rotation Settings")]
        public float rotationSpeed = 5f; // How fast to turn towards player
        public bool onlyRotateY = true; // Lock X and Z rotation

        private float timer;
        private int spawnedSoFar;
        private Transform player;

        public override void Activate()
        {
            base.Activate();
            timer = 0f;
            spawnedSoFar = 0;

            // Cache player reference
            if (OpenWorldEnv.Current != null)
            {
                player = OpenWorldEnv.Current.PlayerTransform;
            }
        }

        public override void Tick()
        {
            if (!Active)
                return;

            // Rotate towards player
            RotateTowardsPlayer();

            // Spawn projectiles
            if (prefabToSpawn != null && spawnPoint != null)
            {
                timer += Time.deltaTime;

                while (timer >= interval)
                {
                    timer -= interval;
                    SpawnObject();
                }
            }
        }

        private void RotateTowardsPlayer()
        {
            if (player == null) return;

            // Calculate direction to player
            Vector3 directionToPlayer = player.position - transform.position;

            if (onlyRotateY)
            {
                // Only rotate on Y axis (horizontal plane)
                directionToPlayer.y = 0f;
            }

            if (directionToPlayer.sqrMagnitude < 0.001f) return;

            // Calculate target rotation
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Smoothly lerp towards target
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        private void SpawnObject()
        {
            // Calculate random rotation BEFORE instantiating
            float randomX = Random.Range(0f, 180f);
            Quaternion spawnRotation = spawnPoint.rotation * Quaternion.Euler(randomX, 0f, 0f);
            
            // Spawn with the correct rotation
            GameObject obj = Object.Instantiate(prefabToSpawn, spawnPoint.position, spawnRotation);

            spawnedSoFar++;
        }
    }
}