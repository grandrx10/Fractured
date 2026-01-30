using UnityEngine;

namespace Game.Bosses
{
    [CreateAssetMenu(menuName = "BossAttacks/Kaien/ActivatePlatforms")]
    public class PlatformActiveAttack : BossAttack
    {
        [Header("Source")]
        public string sourcePointName; // NamedPoint on the boss (optional)

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            Boss bossComponent = boss.GetComponent<Boss>();
            if (bossComponent == null)
                return;

            Transform sourceTransform = boss.transform;

            // Use named point if provided
            if (!string.IsNullOrEmpty(sourcePointName))
            {
                Transform namedPoint = bossComponent.GetPointTransform(sourcePointName);
                if (namedPoint != null)
                    sourceTransform = namedPoint;
                else
                    Debug.LogWarning(
                        $"PlatformActiveAttack: Named point '{sourcePointName}' not found on boss '{boss.name}'."
                    );
            }

            // Activate all platforms under the source transform
            Platform[] platforms = sourceTransform.GetComponentsInChildren<Platform>(true);
            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i].Activate();
            }
        }

        // No Tick logic needed
        public override void Tick(GameObject boss) { }
    }
}
