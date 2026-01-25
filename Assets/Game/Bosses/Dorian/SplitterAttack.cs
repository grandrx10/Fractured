using UnityEngine;

namespace Game.Bosses.Dorian
{
    [CreateAssetMenu(menuName = "BossAttacks/Dorian/SplitterAttack")]
    public class SplitterAttack : BlitzAttack
    {
        [Header("Splitter Settings")]
        public GameObject spawnPrefab;

        protected override void OnDashComplete(GameObject boss)
        {
            if (spawnPrefab == null || boss == null)
                return;

            Object.Instantiate(
                spawnPrefab,
                boss.transform.position,
                boss.transform.rotation
            );
        }
    }
}
