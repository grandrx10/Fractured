using UnityEngine;
using Game.Bosses;

namespace Game.Bosses.Cyra
{
    [CreateAssetMenu(menuName = "BossAttacks/Cyra/SunDanceAttack")]
    public class SunDanceAttack : BossAttack
    {
        [Header("Spawn Settings")]
        [Tooltip("The named boss point where the SunDance object will spawn")]
        public string spawnPointName;

        [Tooltip("The SunDance prefab to spawn")]
        public GameObject sunDancePrefab;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            if (sunDancePrefab == null)
            {
                Debug.LogError("SunDanceAttack: SunDance prefab not assigned!");
                return;
            }

            // Get the spawn transform from the boss
            Transform spawnPoint = boss.GetComponent<Boss>()?.GetPointTransform(spawnPointName);
            if (spawnPoint == null)
            {
                Debug.LogError("SunDanceAttack: Spawn point not found: " + spawnPointName);
                return;
            }

            // Spawn SunDance once
            GameObject sunDance = Instantiate(sunDancePrefab, spawnPoint.position, Quaternion.identity);
        }

        // No ongoing Tick needed
        public override void Tick(GameObject boss) { }

        // EndAttack can remain default
    }
}
