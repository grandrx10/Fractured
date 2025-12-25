using System.Collections;
using UnityEngine;
using Characters;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Dorian
{
    [CreateAssetMenu(menuName = "BossAttacks/Dorian/SummonExplosionAttack")]
    public class SummonExplosionAttack : BossAttack
    {
        [Header("Prefabs")]
        public GameObject projectilePrefab;   // The object to summon above the boss
        public GameObject warningPrefab;      // Warning prefab

        [Header("Attack Settings")]
        public float warningRadius = 2f;
        public float warningDuration = 0.5f;
        public float growDuration = 2f;       // Time to scale up
        public float targetScale = 5f;        // Final scale
        public float destroyDelay = 0.5f;     // Time after full size before destroying projectile

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);
            MonoBehaviour mb = boss.GetComponent<MonoBehaviour>();
            if (mb != null)
                mb.StartCoroutine(AttackSequence(boss));
            else
                Debug.LogError("Boss has no MonoBehaviour component!");
        }

        private IEnumerator AttackSequence(GameObject boss)
        {
            if (!isActive || PlayerSingleton.Instance == null)
                yield break;

            // Spawn warning at boss position
            if (warningPrefab != null)
            {
                Warning w = Object.Instantiate(
                    warningPrefab,
                    boss.transform.position,
                    Quaternion.identity
                ).GetComponent<Warning>();

                if (w != null)
                {
                    w.Initialize((int)warningRadius, warningDuration, Warning.WarningType.Grounded, warningDuration);
                }
            }

            // Wait for the warning duration
            yield return new WaitForSeconds(warningDuration);

            // Spawn projectile above the boss
            if (projectilePrefab != null)
            {
                GameObject proj = Object.Instantiate(projectilePrefab, boss.transform.position, Quaternion.identity);
                proj.transform.localScale = Vector3.zero;

                // Grow to target scale over growDuration
                float elapsed = 0f;
                while (elapsed < growDuration && proj != null)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / growDuration);
                    proj.transform.localScale = Vector3.one * Mathf.Lerp(0f, targetScale, t);
                    yield return null;
                }

                if (proj != null)
                    proj.transform.localScale = Vector3.one * targetScale;

                // Wait a little after reaching full size, then destroy
                if (proj != null)
                    Object.Destroy(proj, destroyDelay);
            }
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);
            // Cleanup handled automatically
        }
    }
}
