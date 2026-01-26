using System.Collections;
using UnityEngine;
using Game.Bosses;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Ravenna
{
    [CreateAssetMenu(menuName = "BossAttacks/Ravenna/HammerAttack")]
    public class HammerAttack : BossAttack
    {
        [Header("Named Point Lists (same length)")]
        public string[] hammerSpawnPoints;
        public string[] mainPlatformPoints;
        public string[] affectedPlatformPoints;

        [Header("Prefabs")]
        public GameObject hammerPrefab;
        public GameObject warningPrefab;
        public GameObject explosionPrefab;

        [Header("Timings")]
        public float attackInterval = 6f;
        public float hammerRotateDuration = 5f;
        public float platformMoveDuration = 2f;

        [Header("Movement")]
        public float platformOffset = 50f;

        [Header("Warning")]
        public int warningRadius = 6;
        public float warningDuration = 2f;

        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            boss.GetComponent<MonoBehaviour>()
                .StartCoroutine(HammerLoop(boss));
        }

        private IEnumerator HammerLoop(GameObject boss)
        {
            while (isActive)
            {
                ExecuteHammerCycle(boss);
                yield return new WaitForSeconds(attackInterval);
            }

            SetTrigger(boss, "next");
        }

        private void ExecuteHammerCycle(GameObject boss)
        {
            if (hammerSpawnPoints.Length == 0 ||
                hammerSpawnPoints.Length != mainPlatformPoints.Length ||
                hammerSpawnPoints.Length != affectedPlatformPoints.Length)
            {
                Debug.LogError("HammerAttack: Point lists are invalid or mismatched.");
                return;
            }

            int index = Random.Range(0, hammerSpawnPoints.Length);

            Transform hammerSpawn = boss.GetComponent<Boss>()
                .GetPointTransform(hammerSpawnPoints[index]);
            Transform mainPlatform = boss.GetComponent<Boss>()
                .GetPointTransform(mainPlatformPoints[index]);
            Transform affectedPlatform = boss.GetComponent<Boss>()
                .GetPointTransform(affectedPlatformPoints[index]);

            if (hammerSpawn == null || mainPlatform == null || affectedPlatform == null)
                return;

            boss.GetComponent<MonoBehaviour>().StartCoroutine(
                HammerSequence(
                    hammerSpawn,
                    mainPlatform,
                    affectedPlatform
                )
            );
        }

        private IEnumerator HammerSequence(
            Transform hammerSpawn,
            Transform mainPlatform,
            Transform affectedPlatform)
        {
            Vector3 mainStartPos = mainPlatform.position;
            Vector3 affectedStartPos = affectedPlatform.position;

            // Spawn warning on main platform
            if (warningPrefab != null)
            {
                Warning w = Instantiate(
                    warningPrefab,
                    mainPlatform.position,
                    Quaternion.identity
                ).GetComponent<Warning>();

                if (w != null)
                {
                    w.Initialize(
                        warningRadius,
                        warningDuration,
                        Warning.WarningType.Grounded,
                        warningDuration
                    );
                    Destroy(w.gameObject, warningDuration);
                }

                Warning w2 = Instantiate(
                    warningPrefab,
                    affectedPlatform.position,
                    Quaternion.identity
                ).GetComponent<Warning>();

                if (w2 != null)
                {
                    w2.Initialize(
                        warningRadius,
                        warningDuration,
                        Warning.WarningType.Grounded,
                        warningDuration
                    );
                    Destroy(w2.gameObject, warningDuration);
                }
            }

            // Ensure warning finishes BEFORE hammer begins
            yield return new WaitForSeconds(warningDuration);
            // Spawn hammer and parent it to the spawn transform
            GameObject hammer = Instantiate(
                hammerPrefab,
                hammerSpawn.position,
                hammerSpawn.rotation,
                hammerSpawn
            );

            // Rotate hammer 90 degrees on Z
            Quaternion startRot = hammer.transform.localRotation;
            Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, 90f);

            yield return LerpRotation(
                hammer.transform,
                startRot,
                endRot,
                hammerRotateDuration
            );
            GameObject explosion = Instantiate(
                explosionPrefab,
                affectedPlatform.position,
                Quaternion.identity,
                affectedPlatform // parent to the affected platform
            );

            // Move platforms
            Vector3 mainEnd = mainStartPos - Vector3.up * platformOffset;
            Vector3 affectedEnd = affectedStartPos + Vector3.up * platformOffset;

            yield return LerpPlatforms(
                mainPlatform,
                affectedPlatform,
                mainStartPos,
                mainEnd,
                affectedStartPos,
                affectedEnd,
                platformMoveDuration
            );

            // Cleanup hammer
            if (hammer != null)
                Destroy(hammer);

            // Return platforms
            yield return LerpPlatforms(
                mainPlatform,
                affectedPlatform,
                mainEnd,
                mainStartPos,
                affectedEnd,
                affectedStartPos,
                platformMoveDuration
            );
        }

        private IEnumerator LerpRotation(
            Transform t,
            Quaternion start,
            Quaternion end,
            float duration)
        {
            float time = 0f;
            while (time < duration && t != null)
            {
                t.localRotation = Quaternion.Slerp(start, end, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            if (t != null)
                t.localRotation = end;
        }

        private IEnumerator LerpPlatforms(
            Transform main,
            Transform affected,
            Vector3 mainStart,
            Vector3 mainEnd,
            Vector3 affectedStart,
            Vector3 affectedEnd,
            float duration)
        {
            float time = 0f;
            while (time < duration)
            {
                float t = time / duration;

                if (main != null)
                    main.position = Vector3.Lerp(mainStart, mainEnd, t);

                if (affected != null)
                    affected.position = Vector3.Lerp(affectedStart, affectedEnd, t);

                time += Time.deltaTime;
                yield return null;
            }

            if (main != null)
                main.position = mainEnd;

            if (affected != null)
                affected.position = affectedEnd;
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);
        }
    }
}
