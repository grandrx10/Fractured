using System.Collections.Generic;
using Cards.Environments;
using UnityEngine;

namespace Game.Bosses
{
    [System.Serializable]
    public class BossPhase
    {
        public string phaseName;
        public BossAttack[] attacks;

        [Header("Phase End Conditions")]
        public int minHealthThreshold;   // Used if phaseTime <= 0
        public float phaseTime = 0f;     // If > 0, phase ends after this time
    }

    [System.Serializable]
    public class NamedPoint
    {
        public string name;
        public Transform transform;
    }

    public class Boss : MonoBehaviour
    {
        [Header("Phases")]
        public BossPhase[] phases;

        [Header("Health")]
        public Health.Health bossHealth;

        [Header("Named Points")]
        public NamedPoint[] namedPoints;

        private int currentPhaseIndex = 0;
        private int currentAttackIndex = 0;

        private float attackTimer = 0f;
        private float delayTimer = 0f;
        private float phaseTimer = 0f;

        private bool waitingForNextAttack = false;
        private bool phaseEndRequested = false;
        private bool _initialized;
        // Tracks trigger-once attacks globally (per your current design)
        private HashSet<BossAttack> triggeredOnceAttacks = new HashSet<BossAttack>();

        private void Awake()
        {
            GlobalWorldManager.OnLoadNewScene += Init;
        }
        
        private void Init(CardEnv environment)
        {
            GlobalWorldManager.OnLoadNewScene -= Init;
            if (phases.Length == 0)
                return;
            _initialized = true;
            Debug.Log("bos start");
            StartPhase(0);
        }

        private void Update()
        {
            if (phases.Length == 0 || !_initialized)
                return;
        
            BossPhase phase = phases[currentPhaseIndex];

            // =========================
            // Phase Timer (request end)
            // =========================
            if (phase.phaseTime > 0f && !phaseEndRequested)
            {
                phaseTimer += Time.deltaTime;
                if (phaseTimer >= phase.phaseTime)
                    phaseEndRequested = true;
            }

            if (phase.attacks.Length == 0)
                return;

            BossAttack attack = phase.attacks[currentAttackIndex];

            if (!waitingForNextAttack)
            {
                attack.Tick(gameObject);

                if (!attack.manualSkip)
                {
                    attackTimer += Time.deltaTime;
                    if (attackTimer >= attack.attackDuration)
                        EndCurrentAttack();
                }
            }
            else
            {
                delayTimer += Time.deltaTime;
                if (delayTimer >= attack.delayAfter)
                    AdvanceToNextAttack();
            }

            // =========================
            // Health-Based Phase Check
            // =========================
            if (phase.phaseTime <= 0f && !phaseEndRequested)
                CheckPhaseSwitch();
        }

        // =========================
        // Phase Management
        // =========================

        private void StartPhase(int phaseIndex)
        {
            currentPhaseIndex = phaseIndex;
            currentAttackIndex = 0;

            attackTimer = 0f;
            delayTimer = 0f;
            phaseTimer = 0f;
            phaseEndRequested = false;
            waitingForNextAttack = false;

            BossPhase phase = phases[currentPhaseIndex];
            if (phase.attacks.Length > 0)
                StartAttack(phase.attacks[0]);
        }

        private void AdvanceToNextPhase()
        {
            if (currentPhaseIndex >= phases.Length - 1)
                return;

            StartPhase(currentPhaseIndex + 1);
        }

        private void CheckPhaseSwitch()
        {
            BossPhase phase = phases[currentPhaseIndex];
            if (bossHealth.health > phase.minHealthThreshold)
                return;

            phaseEndRequested = true;
        }

        // =========================
        // Attack Management
        // =========================

        private void StartAttack(BossAttack attack)
        {
            // Never start new attacks while the phase is ending
            if (attack == null || phaseEndRequested)
                return;

            if (attack.triggerOnce && triggeredOnceAttacks.Contains(attack))
            {
                AdvanceToNextAttack();
                return;
            }

            attackTimer = 0f;
            delayTimer = 0f;
            waitingForNextAttack = false;

            if (attack.triggerOnce)
                triggeredOnceAttacks.Add(attack);

            attack.StartAttack(gameObject);
        }

        public void EndCurrentAttack()
        {
            BossPhase phase = phases[currentPhaseIndex];
            BossAttack attack = phase.attacks[currentAttackIndex];

            attack.EndAttack(gameObject);

            attackTimer = 0f;
            delayTimer = 0f;
            waitingForNextAttack = true;

            // CRITICAL: decide trigger-once exhaustion HERE (correct timing)
            if (!phaseEndRequested && AllTriggerOnceAttacksUsed(phase))
            {
                phaseEndRequested = true;
            }

            // Phase transition ONLY after attack ends
            if (phaseEndRequested)
                AdvanceToNextPhase();
        }

        private void AdvanceToNextAttack()
        {
            // Freeze cycling if phase is ending
            if (phaseEndRequested)
                return;

            BossPhase phase = phases[currentPhaseIndex];
            if (phase.attacks.Length == 0)
                return;

            currentAttackIndex = (currentAttackIndex + 1) % phase.attacks.Length;
            StartAttack(phase.attacks[currentAttackIndex]);
        }

        private bool AllTriggerOnceAttacksUsed(BossPhase phase)
        {
            for (int i = 0; i < phase.attacks.Length; i++)
            {
                BossAttack a = phase.attacks[i];
                if (!a.triggerOnce)
                    return false;

                if (!triggeredOnceAttacks.Contains(a))
                    return false;
            }
            return true;
        }

        // =========================
        // Named Points
        // =========================

        public Vector3 GetPointPosition(string pointName)
        {
            foreach (var np in namedPoints)
            {
                if (np.name == pointName && np.transform != null)
                    return np.transform.position;
            }
            return transform.position;
        }

        public Transform GetPointTransform(string pointName)
        {
            foreach (var np in namedPoints)
            {
                if (np.name == pointName && np.transform != null)
                    return np.transform;
            }
            return null;
        }

        public void SetPointTransform(string pointName, Transform newTransform)
        {
            for (int i = 0; i < namedPoints.Length; i++)
            {
                if (namedPoints[i].name == pointName)
                {
                    namedPoints[i].transform = newTransform;
                    return;
                }
            }

            // If the point doesn't exist yet, optionally add it
            List<NamedPoint> list = new List<NamedPoint>(namedPoints);
            list.Add(new NamedPoint { name = pointName, transform = newTransform });
            namedPoints = list.ToArray();
        }
    }
}
