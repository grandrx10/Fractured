using System.Collections.Generic;
using UnityEngine;

namespace Game.Bosses
{
    [System.Serializable]
    public class BossPhase
    {
        public string phaseName;
        public BossAttack[] attacks;
        public int minHealthThreshold; // Switch to next phase when health drops below this
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
        private bool waitingForNextAttack = false;

        private HashSet<BossAttack> triggeredOnceAttacks = new HashSet<BossAttack>();

        private void Start()
        {
            if (phases.Length == 0 || phases[0].attacks.Length == 0)
                return;

            StartAttack(phases[0].attacks[0]);
        }

        private void Update()
        {
            if (phases.Length == 0)
                return;

            BossPhase phase = phases[currentPhaseIndex];
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

            CheckPhaseSwitch();
        }

        private void StartAttack(BossAttack attack)
        {
            if (attack == null)
                return;

            if (attack.triggerOnce && triggeredOnceAttacks.Contains(attack))
            {
                AdvanceToNextAttack();
                return;
            }

            attackTimer = 0f;
            delayTimer = 0f;
            waitingForNextAttack = false;

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
        }

        private void AdvanceToNextAttack()
        {
            BossPhase phase = phases[currentPhaseIndex];

            currentAttackIndex = (currentAttackIndex + 1) % phase.attacks.Length;
            StartAttack(phase.attacks[currentAttackIndex]);
        }

        private void CheckPhaseSwitch()
        {
            BossPhase phase = phases[currentPhaseIndex];

            if (bossHealth.health > phase.minHealthThreshold)
                return;

            if (currentPhaseIndex >= phases.Length - 1)
                return;

            currentPhaseIndex++;
            currentAttackIndex = 0;

            attackTimer = 0f;
            delayTimer = 0f;
            waitingForNextAttack = false;

            BossPhase newPhase = phases[currentPhaseIndex];
            if (newPhase.attacks.Length > 0)
                StartAttack(newPhase.attacks[0]);
        }

        // ================================
        // Named Points
        // ================================

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
    }
}
