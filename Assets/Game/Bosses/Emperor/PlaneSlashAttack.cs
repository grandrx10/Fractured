using System.Collections;
using UnityEngine;
using Cards.Environments;
using Game.Bosses.Projectiles;

namespace Game.Bosses
{
    [CreateAssetMenu(menuName = "BossAttacks/Emperor/PlaneSlashAttack")]
    public class PlaneSlashAttack : BossAttack
    {
        [Header("Prefabs")]
        public GameObject warningPrefab;      // warning object that fades in
        public GameObject mainHitPrefab;      // plane cut object that replaces warning

        [Header("Warning Settings")]
        public float warningDuration = 1f;
        public float warningFadeTime = 0.3f;

        [Header("Rotation")]
        public Vector2 xRotationRange = new Vector2(60f, 120f);

        [Header("Attack Interval")]
        public float initialInterval = 2f;    // starting interval between slashes
        private float currentInterval;

        private Coroutine attackCoroutine;

        public override void StartAttack(GameObject bossObj)
        {
            base.StartAttack(bossObj);

            Boss boss = bossObj.GetComponent<Boss>();
            if (boss == null)
            {
                Debug.LogError("PlaneSlashAttack: Boss component missing.");
                return;
            }

            currentInterval = initialInterval;
            attackCoroutine = boss.StartCoroutine(AttackRoutine(boss));
        }

        public override void EndAttack(GameObject bossObj)
        {
            base.EndAttack(bossObj);

            if (attackCoroutine != null)
            {
                Boss boss = bossObj.GetComponent<Boss>();
                if (boss != null)
                {
                    boss.StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }
            }
        }

        private IEnumerator AttackRoutine(Boss boss)
        {
            Transform player = OpenWorldEnv.Current.PlayerTransform;

            while (true)
            {
                // Random rotations
                float xRotation = Random.Range(xRotationRange.x, xRotationRange.y);
                float yRotation = Random.Range(0f, 360f);
                Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0f);

                // Spawn warning object
                GameObject warning = Object.Instantiate(
                    warningPrefab,
                    player.position,
                    rotation
                );

                // Start individual attack coroutine for this slash
                boss.StartCoroutine(HandleSingleSlash(warning));

                // Wait for current interval before next slash (happens in parallel)
                yield return new WaitForSeconds(currentInterval);

                // Reduce interval for next slash
                currentInterval *= 0.9f;
            }
        }

        private IEnumerator HandleSingleSlash(GameObject warning)
        {
            // Fade in the warning
            yield return FadeIn(warning, warningFadeTime);

            // Keep warning visible for remaining duration
            yield return new WaitForSeconds(warningDuration - warningFadeTime);

            // Store warning's exact position and rotation
            Vector3 warningPosition = warning.transform.position;
            Quaternion warningRotation = warning.transform.rotation;

            // Destroy warning and spawn main hit object at exact same transform
            Object.Destroy(warning);
            
            GameObject mainHit = Object.Instantiate(
                mainHitPrefab,
                warningPosition,
                warningRotation
            );
        }

        private IEnumerator FadeIn(GameObject obj, float duration)
        {
            // Get all renderers in the object and its children
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            
            // Store original colors
            Color[][] originalColors = new Color[renderers.Length][];
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] materials = renderers[i].materials;
                originalColors[i] = new Color[materials.Length];
                
                for (int j = 0; j < materials.Length; j++)
                {
                    originalColors[i][j] = materials[j].color;
                    
                    // Start fully transparent
                    Color transparent = materials[j].color;
                    transparent.a = 0f;
                    materials[j].color = transparent;
                }
            }

            // Fade in
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                for (int i = 0; i < renderers.Length; i++)
                {
                    Material[] materials = renderers[i].materials;
                    for (int j = 0; j < materials.Length; j++)
                    {
                        Color color = originalColors[i][j];
                        color.a = Mathf.Lerp(0f, originalColors[i][j].a, t);
                        materials[j].color = color;
                    }
                }

                yield return null;
            }

            // Ensure fully opaque at end
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] materials = renderers[i].materials;
                for (int j = 0; j < materials.Length; j++)
                {
                    materials[j].color = originalColors[i][j];
                }
            }
        }
    }
}