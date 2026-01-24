using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cards.Environments;

namespace Game.Bosses
{
    [CreateAssetMenu(menuName = "BossAttacks/Emperor/UnderworldOutAttack")]
    public class UnderworldOutAttack : BossAttack
    {
        [Header("Underworld Sword Settings")]
        public GameObject underworldSwordPrefab;
        public float spawnRadius = 1f;
        public float spawnInterval = 0.5f;
        public float reverseTime = 10f;

        [Header("Circle Spawn")]
        public int swordsPerWave = 8; // evenly spaced
        public bool randomRotation = true;

        private List<UnderworldSword> activeSwords = new();
        private bool spawningEnabled;
        private Boss bossScript;
        private Coroutine routine;

        public override void StartAttack(GameObject bossObj)
        {
            base.StartAttack(bossObj);

            bossScript = bossObj.GetComponent<Boss>();
            if (bossScript == null || underworldSwordPrefab == null)
                return;

            activeSwords.Clear();
            spawningEnabled = true;

            routine = bossScript.StartCoroutine(AttackRoutine());
        }

        public override void EndAttack(GameObject bossObj)
        {
            base.EndAttack(bossObj);

            // IMPORTANT:
            // We STOP SPAWNING, but DO NOT cancel the routine
            spawningEnabled = false;
        }

        private IEnumerator AttackRoutine()
        {
            float elapsed = 0f;

            // Spawn loop
            while (elapsed < reverseTime)
            {
                if (spawningEnabled)
                {
                    SpawnCircle();
                }

                yield return new WaitForSeconds(spawnInterval);
                elapsed += spawnInterval;
            }

            // Reverse ALWAYS happens
            ReverseSwords();
        }

        private void SpawnCircle()
        {
            var env = OpenWorldEnv.Current;
            if (env == null || bossScript == null)
                return;

            Vector3 bossPos = bossScript.transform.position;
            float groundY = env.GetBossTargetGrounded().y - 1f;

            float angleOffset = randomRotation
                ? Random.Range(0f, 360f)
                : 0f;

            for (int i = 0; i < swordsPerWave; i++)
            {
                float angle = angleOffset + (360f / swordsPerWave) * i;
                float rad = angle * Mathf.Deg2Rad;

                Vector3 dir = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));
                Vector3 spawnPos = bossPos + dir * spawnRadius;
                spawnPos.y = groundY;

                Quaternion rot = Quaternion.LookRotation(dir);

                GameObject obj = Instantiate(
                    underworldSwordPrefab,
                    spawnPos,
                    rot
                );

                var sword = obj.GetComponent<UnderworldSword>();
                if (sword != null)
                    activeSwords.Add(sword);
            }
        }

        private void ReverseSwords()
        {
            foreach (var sword in activeSwords)
            {
                if (sword != null)
                    sword.Reverse();
            }
        }
    }
}
