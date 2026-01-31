using UnityEngine;
using System.Collections.Generic;

namespace Game.Bosses
{
    [CreateAssetMenu(fileName = "UnderworldAttack", menuName = "BossAttacks/Emperor/UnderworldAttack")]
    public class UnderworldAttack : BossAttack
    {
        [Header("Underworld Sword Settings")]
        public GameObject underworldSwordPrefab;
        public float spawnRadius = 10f;
        public float spawnInterval = 0.5f;
        public float reverseTime = 10f;

        private List<UnderworldSword> activeSwords = new List<UnderworldSword>();
        private float spawnTimer = 0f;
        private float attackElapsedTime = 0f;
        private bool hasReversed = false;
        private Boss bossScript;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            activeSwords.Clear();
            spawnTimer = 0f;
            attackElapsedTime = 0f;
            hasReversed = false;
            bossScript = boss.GetComponent<Boss>();
        }

        public override void Tick(GameObject boss)
        {
            base.Tick(boss);

            if (underworldSwordPrefab == null)
                return;

            attackElapsedTime += Time.deltaTime;

            // Reverse all swords after reverseTime and stop spawning
            if (attackElapsedTime >= reverseTime && !hasReversed)
            {
                ReverseSwords();
                hasReversed = true;
            }

            // Only spawn swords before reversing
            if (!hasReversed)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= spawnInterval)
                {
                    SpawnUnderworldSword();
                    spawnTimer = 0f;
                }
            }

            // Clean up destroyed swords from list
            activeSwords.RemoveAll(sword => sword == null);
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);

            // Clean up all active UnderworldSwords
            foreach (var sword in activeSwords)
            {
                if (sword != null)
                {
                    Destroy(sword.gameObject);
                }
            }
            activeSwords.Clear();
        }

        private void SpawnUnderworldSword()
        {
            var openWorldEnv = Cards.Environments.OpenWorldEnv.Current;
            if (openWorldEnv == null) return;

            // Get player's ground level position
            Vector3 playerGroundPos = openWorldEnv.GetBossTargetGrounded();
            Vector3 playerPos = openWorldEnv.PlayerPos;

            // Random position within radius around boss
            Vector3 bossPos = bossScript.transform.position;
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = new Vector3(
                bossPos.x + randomCircle.x,
                playerGroundPos.y - 1f,
                bossPos.z + randomCircle.y
            );

            // Calculate direction to player on XZ plane
            Vector3 directionToPlayer = new Vector3(
                playerPos.x - spawnPos.x,
                0,
                playerPos.z - spawnPos.z
            ).normalized;

            Quaternion spawnRotation = Quaternion.LookRotation(directionToPlayer);

            GameObject swordObj = Instantiate(underworldSwordPrefab, spawnPos, spawnRotation);
            UnderworldSword sword = swordObj.GetComponent<UnderworldSword>();

            if (sword != null)
            {
                activeSwords.Add(sword);
            }
        }

        private void ReverseSwords()
        {
            foreach (var sword in activeSwords)
            {
                if (sword != null)
                {
                    sword.Reverse();
                }
            }
        }
    }
}