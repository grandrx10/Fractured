using System.Collections;
using UnityEngine;
using Characters;

namespace Game.Bosses.Dorian
{
    [CreateAssetMenu(menuName = "BossAttacks/Dorian/HollowLightningAttack")]
    public class HollowLightningAttack : BossAttack
    {
        [Header("Transforms")]
        public string arrivalPointName;
        public string spawnPointName;

        [Header("Prefab Settings")]
        public GameObject projectilePrefab;
        public float maxScale = 3f;
        public float growDuration = 2f;
        public float shrinkDuration = 1f;

        [Header("Movement Settings")]
        public float lerpDuration = 1f;
        public float turnSpeed = 180f;
        public float moveSpeed = 10f;

        private GameObject activeProjectile;
        private MonoBehaviour context;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            context = boss.GetComponent<MonoBehaviour>();
            if (context != null)
                context.StartCoroutine(AttackSequence(boss));
            else
                Debug.LogError("Boss has no MonoBehaviour component!");
        }

        private IEnumerator AttackSequence(GameObject boss)
        {
            Transform arrivalPoint = boss.GetComponent<Boss>()?.GetPointTransform(arrivalPointName) ?? boss.transform;
            Transform spawnPoint = boss.GetComponent<Boss>()?.GetPointTransform(spawnPointName) ?? boss.transform;

            // Phase 1: Lerp boss to arrival point
            Vector3 startPos = boss.transform.position;
            Vector3 targetPos = arrivalPoint.position;
            float elapsed = 0f;

            while (elapsed < lerpDuration && isActive)
            {
                elapsed += Time.deltaTime;
                boss.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / lerpDuration);
                yield return null;
            }
            boss.transform.position = targetPos;

            // Phase 2: Spawn projectile
            if (projectilePrefab != null && context != null)
            {
                Quaternion initialRotation = spawnPoint.rotation;

                // 🔹 Aim at player immediately
                if (PlayerSingleton.Instance != null)
                {
                    Vector3 dir = PlayerSingleton.Instance.transform.position - spawnPoint.position;
                    if (dir != Vector3.zero)
                        initialRotation = Quaternion.LookRotation(dir);
                }

                activeProjectile = Object.Instantiate(
                    projectilePrefab,
                    spawnPoint.position,
                    initialRotation
                );

                activeProjectile.transform.localScale = Vector3.zero;

                // Grow projectile
                elapsed = 0f;
                while (elapsed < growDuration && activeProjectile != null && isActive)
                {
                    elapsed += Time.deltaTime;
                    activeProjectile.transform.localScale =
                        Vector3.Lerp(Vector3.zero, Vector3.one * maxScale, elapsed / growDuration);
                    yield return null;
                }

                if (activeProjectile != null)
                    activeProjectile.transform.localScale = Vector3.one * maxScale;

                if (activeProjectile != null)
                    context.StartCoroutine(TrackPlayer(activeProjectile));
            }
        }

        private IEnumerator TrackPlayer(GameObject proj)
        {
            if (proj == null || context == null)
                yield break;

            while (proj != null && PlayerSingleton.Instance != null && isActive)
            {
                Transform playerTransform = PlayerSingleton.Instance.transform;
                float playerY = playerTransform.position.y;

                Vector3 direction = (playerTransform.position - proj.transform.position).normalized;

                if (direction != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(direction);
                    proj.transform.rotation = Quaternion.RotateTowards(
                        proj.transform.rotation,
                        targetRot,
                        turnSpeed * Time.deltaTime
                    );
                }

                // Move forward
                proj.transform.position += proj.transform.forward * moveSpeed * Time.deltaTime;

                // Clamp Y so it never goes below player
                Vector3 pos = proj.transform.position;
                pos.y = Mathf.Max(pos.y, playerY);
                proj.transform.position = pos;

                yield return null;
            }
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);

            if (activeProjectile != null && context != null)
            {
                context.StartCoroutine(ShrinkAndDestroy(activeProjectile));
                activeProjectile = null;
            }
        }

        private IEnumerator ShrinkAndDestroy(GameObject proj)
        {
            if (proj == null)
                yield break;

            Vector3 startScale = proj.transform.localScale;
            float elapsed = 0f;

            while (elapsed < shrinkDuration)
            {
                elapsed += Time.deltaTime;
                proj.transform.localScale =
                    Vector3.Lerp(startScale, Vector3.zero, elapsed / shrinkDuration);
                yield return null;
            }

            Object.Destroy(proj);
        }
    }
}
