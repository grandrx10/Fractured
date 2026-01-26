using System.Collections;
using UnityEngine;
using Cards.Environments;
using Characters;
using Game.Bosses;
using Game.Bosses.Projectiles;

namespace Game.Bosses.Ravenna
{
    [CreateAssetMenu(menuName = "BossAttacks/Ravenna/WingsAttackHard")]
    public class WingsAttackHard : BossAttack
    {
        /* =========================
         * WINGS ATTACK (BASE)
         * ========================= */
        [Header("Wing Spawn Settings")]
        public GameObject wingProjectilePrefab;
        public string[] wingPointNames;
        public float wingSpawnInterval = 1f;

        [Header("Pulse Settings")]
        public float onDuration = 3f;
        public float offDuration = 3f;

        [Header("Movement Settings")]
        public string[] movePoints;
        public float moveDuration = 1f;

        private float _wingSpawnTimer;
        private float _pulseTimer;
        private bool _spawningEnabled;
        private bool _moving;
        private bool _waitingForNextCycle;

        /* =========================
         * FEATHERBURST ADDON
         * ========================= */
        [Header("Featherburst Settings")]
        public GameObject featherburstProjectilePrefab;
        public string featherSpawnPointName;
        public int featherCount = 12;
        public float featherSpawnStagger = 0.05f;
        public float featherTargetRadius = 6f;

        [Header("Featherburst Homing")]
        public float featherMoveSpeed = 12f;
        public float featherTurnSpeed = 6f;
        public float featherKillDistance = 0.75f;
        public float speedVariance = 2f;
        public float turnSpeedVariance = 2f;

        [Header("Featherburst Warning")]
        public GameObject warningPrefab;
        public int warningRadius = 3;
        public float warningDuration = 1.5f;

        private Transform _featherSpawnPoint;

        private float _featherSpawnTimer;
        private int _feathersSpawnedThisPulse;
        private Vector3 _currentFeatherTarget;

        /* =========================
         * START / END
         * ========================= */
        public override void StartAttack(GameObject boss)
        {
            base.StartAttack(boss);

            _wingSpawnTimer = 0f;
            _featherSpawnTimer = 0f;
            _pulseTimer = 0f;
            _spawningEnabled = false;
            _moving = false;
            _waitingForNextCycle = false;
            _feathersSpawnedThisPulse = 0;

            Boss bossComponent = boss.GetComponent<Boss>();
            if (bossComponent != null)
                _featherSpawnPoint = bossComponent.GetPointTransform(featherSpawnPointName);

            MoveToRandomPoint(boss);

            NpcCommands npc = boss.GetComponent<NpcCommands>();
            if (npc != null && OpenWorldEnv.Current != null)
                npc.SetLookingAt(OpenWorldEnv.Current.PlayerTransform);
        }

        public override void EndAttack(GameObject boss)
        {
            base.EndAttack(boss);

            NpcCommands npc = boss.GetComponent<NpcCommands>();
            if (npc != null)
                npc.SetLookingAt(null);
        }

        /* =========================
         * TICK
         * ========================= */
        public override void Tick(GameObject boss)
        {
            if (!isActive || _moving || _waitingForNextCycle)
                return;

            if (_spawningEnabled)
            {
                // --- Wing spawning ---
                _wingSpawnTimer += Time.deltaTime;
                if (_wingSpawnTimer >= wingSpawnInterval)
                {
                    _wingSpawnTimer -= wingSpawnInterval;
                    SpawnWingProjectiles(boss);
                }

                // --- Featherburst spawning ---
                _featherSpawnTimer += Time.deltaTime;
                if (_featherSpawnTimer >= featherSpawnStagger)
                {
                    _featherSpawnTimer -= featherSpawnStagger;
                    SpawnFeatherBurstStep(boss);
                }

                // --- Pulse timing ---
                _pulseTimer += Time.deltaTime;
                if (_pulseTimer >= onDuration)
                {
                    _spawningEnabled = false;
                    _pulseTimer = 0f;
                    _featherSpawnTimer = 0f;
                    _feathersSpawnedThisPulse = 0;
                    boss.GetComponent<Boss>().StartCoroutine(OffPhaseCoroutine(boss));
                }
            }
        }

        /* =========================
         * MOVEMENT
         * ========================= */
        private void MoveToRandomPoint(GameObject boss)
        {
            if (movePoints == null || movePoints.Length == 0 || _moving)
                return;

            Boss bossComponent = boss.GetComponent<Boss>();
            if (bossComponent == null)
                return;

            string pointName = movePoints[Random.Range(0, movePoints.Length)];
            Transform target = bossComponent.GetPointTransform(pointName);
            if (target != null)
            {
                bossComponent.StartCoroutine(
                    MoveToPointCoroutine(boss, target.position)
                );
            }
        }

        private IEnumerator MoveToPointCoroutine(GameObject boss, Vector3 targetPos)
        {
            _moving = true;
            float elapsed = 0f;
            Vector3 start = boss.transform.position;

            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                boss.transform.position = Vector3.Lerp(start, targetPos, elapsed / moveDuration);
                yield return null;
            }

            boss.transform.position = targetPos;
            _moving = false;

            _spawningEnabled = true;
            _wingSpawnTimer = 0f;
            _pulseTimer = 0f;
            _featherSpawnTimer = 0f;
            _feathersSpawnedThisPulse = 0;
        }

        private IEnumerator OffPhaseCoroutine(GameObject boss)
        {
            _waitingForNextCycle = true;
            yield return new WaitForSeconds(offDuration);
            _waitingForNextCycle = false;
            MoveToRandomPoint(boss);
        }

        /* =========================
         * WING PROJECTILES
         * ========================= */
        private void SpawnWingProjectiles(GameObject boss)
        {
            if (wingProjectilePrefab == null)
                return;

            Boss bossComponent = boss.GetComponent<Boss>();
            if (bossComponent == null)
                return;

            foreach (string pointName in wingPointNames)
            {
                Transform t = bossComponent.GetPointTransform(pointName);
                if (t == null)
                    continue;

                float tilt = Random.Range(-15f, 15f);
                Quaternion rot = t.rotation * Quaternion.Euler(tilt, 0f, 0f);
                Instantiate(wingProjectilePrefab, t.position, rot);
            }
        }

        /* =========================
         * FEATHERBURST
         * ========================= */
        private void SpawnFeatherBurstStep(GameObject boss)
        {
            if (_featherSpawnPoint == null || featherburstProjectilePrefab == null)
                return;

            // Initialize warning and target once per pulse
            if (_feathersSpawnedThisPulse == 0 && warningPrefab != null)
            {
                Vector3 playerGround = OpenWorldEnv.Current.GetBossTargetGrounded();
                Vector2 offset = Random.insideUnitCircle * featherTargetRadius;
                _currentFeatherTarget = playerGround + new Vector3(offset.x, 0f, offset.y);

                Warning w = Instantiate(
                    warningPrefab,
                    _currentFeatherTarget,
                    Quaternion.identity
                ).GetComponent<Warning>();

                w.Initialize(
                warningRadius,
                warningDuration,           // ← use warningDuration, not onDuration
                Warning.WarningType.Grounded,
                warningDuration
            );
            }

            if (_feathersSpawnedThisPulse < featherCount)
            {
                SpawnFeather(_currentFeatherTarget);
                _feathersSpawnedThisPulse++;
            }
            else
            {
                // Reset for next pulse
                _feathersSpawnedThisPulse = 0;
            }
        }

        private void SpawnFeather(Vector3 targetPos)
        {
            Quaternion rot =
                _featherSpawnPoint.rotation *
                Quaternion.Euler(
                    Random.Range(-15f, 15f),
                    Random.Range(-15f, 15f),
                    0f
                );

            GameObject proj = Instantiate(
                featherburstProjectilePrefab,
                _featherSpawnPoint.position,
                rot
            );

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb == null)
                rb = proj.AddComponent<Rigidbody>();

            rb.useGravity = false;
            rb.linearDamping = 0f;

            FeatherHoming homing =
                proj.GetComponent<FeatherHoming>() ??
                proj.AddComponent<FeatherHoming>();

            homing.Initialize(
                targetPos,
                featherMoveSpeed + Random.Range(-speedVariance, speedVariance),
                featherTurnSpeed + Random.Range(-turnSpeedVariance, turnSpeedVariance),
                featherKillDistance
            );
        }
    }
}
