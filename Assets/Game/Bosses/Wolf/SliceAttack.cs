using System.Collections;
using Cards.Environments;
using Characters;
using UnityEngine;

namespace Game.Bosses.Wolf
{
    [CreateAssetMenu(menuName = "BossAttacks/Wolf/SliceAttack")]
    public class SliceAttack : BossAttack
    {
        [Header("Spawn Settings")]
        public string spawnPointName;

        [Header("Attack Settings")]
        public float attackInterval = 0.5f;

        [Header("Projectile Settings")]
        public GameObject projectilePrefab;

        private Transform spawnPoint;
        [Header("Aim Variation")]
        public float minAngleOffset = 0f;
        public float maxAngleOffset = 10f;


        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            spawnPoint = boss.GetComponent<Boss>()
                .GetPointTransform(spawnPointName);

            if (spawnPoint == null)
            {
                Debug.LogError($"SliceAttack: Spawn point not found: {spawnPointName}");
                return;
            }

            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null)
            {
                npcCommands.SetLookingAt(OpenWorldEnv.Current.PlayerTransform);
            }

            boss.GetComponent<MonoBehaviour>()
                .StartCoroutine(FireLoop());
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);

            NpcCommands npcCommands = boss.GetComponent<NpcCommands>();
            if (npcCommands != null)
            {
                npcCommands.SetLookingAt(null);
            }
        }

        private IEnumerator FireLoop()
        {
            while (isActive)
            {
                FireProjectile();
                yield return new WaitForSeconds(attackInterval);
            }
        }

        private void FireProjectile()
        {
            Vector3 spawnPos = spawnPoint.position;
            Vector3 targetPos = OpenWorldEnv.Current.PlayerPos;

            // Base aim direction
            Vector3 direction = (targetPos - spawnPos).normalized;

            // ----- Cone firing -----
            float coneAngle = Random.Range(minAngleOffset, maxAngleOffset);

            Vector3 randomAxis = Vector3.Cross(direction, Random.onUnitSphere).normalized;
            if (randomAxis == Vector3.zero)
                randomAxis = Vector3.up;

            Quaternion coneRotation = Quaternion.AngleAxis(coneAngle, randomAxis);
            Vector3 finalDirection = coneRotation * direction;
            // -----------------------

            // ----- Random forward roll (visual only) -----
            float rollAngle = Random.Range(0f, 360f);
            Quaternion rollRotation = Quaternion.AngleAxis(rollAngle, Vector3.forward);
            // --------------------------------------------

            Quaternion finalRotation =
                Quaternion.LookRotation(finalDirection) * rollRotation;

            Instantiate(
                projectilePrefab,
                spawnPos,
                finalRotation
            );
        }


    }
}
