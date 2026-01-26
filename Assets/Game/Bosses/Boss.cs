using System.Collections.Generic;
using Cards.Environments;
using UnityEngine;
using System.Collections;
using Characters.Dialogue;

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

        [Header("Death Dialogue")]
        public string deathConversationName;

        private int currentPhaseIndex = 0;
        private int currentAttackIndex = 0;

        private float attackTimer = 0f;
        private float delayTimer = 0f;
        private float phaseTimer = 0f;

        private bool waitingForNextAttack = true;
        private bool phaseEndRequested = false;
        private bool _initialized;
        private bool isDead = false;

        public bool ignoreInit = false;
        public bool destroyOnDeath = false;

        // Tracks trigger-once attacks globally
        private HashSet<BossAttack> triggeredOnceAttacks = new HashSet<BossAttack>();

        private void Awake()
        {
            GlobalWorldManager.OnLoadNewScene += Init;

            if (ignoreInit)
            {
                _initialized = true;
                StartPhase(0);
            }
        }

        private void Init(CardEnv environment)
        {
            GlobalWorldManager.OnLoadNewScene -= Init;
            if (phases.Length == 0)
                return;
            _initialized = true;
            Debug.Log("Boss initialized");
            StartPhase(0);
        }

        private void Update()
        {
            if (!_initialized || phases.Length == 0 || isDead)
                return;

            // ======= DEATH CHECK =======
            if (bossHealth.health <= 0 && !isDead)
            {
                isDead = true;
                OnBossDeath();
                return;
            }

            BossPhase phase = phases[currentPhaseIndex];

            // =========================
            // Phase Timer
            // =========================
            if (phase.phaseTime > 0f && !phaseEndRequested)
            {
                phaseTimer += Time.deltaTime;
                if (phaseTimer >= phase.phaseTime)
                {
                    phaseEndRequested = true;
                }
            }

            // =========================
            // Health-Based Phase Check (MOVED UP HERE)
            // =========================
            if (phase.phaseTime <= 0f && !phaseEndRequested)
                CheckPhaseSwitch();

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
            phaseEndRequested = false;  // This needs to be reset BEFORE StartAttack is called
            waitingForNextAttack = false;


            BossPhase phase = phases[currentPhaseIndex];
            if (phase.attacks.Length > 0)
            {
                StartAttack(phase.attacks[0]);
            }
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

            if (!phaseEndRequested && AllTriggerOnceAttacksUsed(phase))
                phaseEndRequested = true;

            if (phaseEndRequested)
            {
                AdvanceToNextPhase();
            }
                
        }

        private void AdvanceToNextAttack()
        {
            if (phaseEndRequested)
            {
                AdvanceToNextPhase();
                return;
            }
            
            BossPhase phase = phases[currentPhaseIndex];
            if (phase.attacks.Length == 0) return;

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

            List<NamedPoint> list = new List<NamedPoint>(namedPoints);
            list.Add(new NamedPoint { name = pointName, transform = newTransform });
            namedPoints = list.ToArray();
        }

        // =========================
        // Boss Death / Dialogue
        // =========================
        private void OnBossDeath()
        {
            Debug.Log("Boss defeated!");

            // Stop current attack
            if (phases.Length > 0 && currentAttackIndex < phases[currentPhaseIndex].attacks.Length)
            {
                BossAttack attack = phases[currentPhaseIndex].attacks[currentAttackIndex];
                attack?.EndAttack(gameObject);
            }

            // Stop timers
            attackTimer = 0f;
            delayTimer = 0f;
            waitingForNextAttack = true;
            phaseEndRequested = true;

            // Trigger death dialogue
            if (!string.IsNullOrEmpty(deathConversationName) && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartConversation(deathConversationName);
            }

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }
}
