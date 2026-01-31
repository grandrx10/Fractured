using UnityEngine;

namespace Game.Bosses.Dorian
{
    [CreateAssetMenu(menuName = "BossAttacks/Dorian/CloneBlitzAttack")]
    public class CloneBlitzAttack : BossAttack
    {
        [Header("Source Attack (Shared Asset)")]
        public BlitzAttack sourceAttack;

        // Runtime clone (per boss / per use)
        private BlitzAttack runtimeClone;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            if (sourceAttack == null)
            {
                Debug.LogError("CloneBlitzAttack: sourceAttack is null.");
                return;
            }

            // 🔹 Clone the ScriptableObject so state is NOT shared
            runtimeClone = Instantiate(sourceAttack);

            // Optional: give it a readable runtime name
            runtimeClone.name = $"{sourceAttack.name}_RuntimeClone";

            runtimeClone.StartAttack(boss);
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);

            if (runtimeClone != null)
            {
                runtimeClone.EndAttack(boss);

                // Destroy the clone explicitly (important!)
                Destroy(runtimeClone);
                runtimeClone = null;
            }
        }
    }
}
