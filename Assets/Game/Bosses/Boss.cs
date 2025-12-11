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
        public BossPhase[] phases;
        public Health.Health bossHealth;

        public NamedPoint[] namedPoints; // List of spawn points

        private int currentPhaseIndex = 0;
        private int currentAttackIndex = 0;
        private float attackTimer = 0f;
        private float delayTimer = 0f;
        private bool waitingForNextAttack = false;

        void Start()
        {
            if (phases.Length > 0 && phases[0].attacks.Length > 0)
                phases[0].attacks[0].StartAttack(gameObject);
        }

        void Update()
        {
            if (phases.Length == 0) return;
            var phase = phases[currentPhaseIndex];
            if (phase.attacks.Length == 0) return;

            var attack = phase.attacks[currentAttackIndex];

            if (!waitingForNextAttack)
            {
                attack.Tick(gameObject);
                attackTimer += Time.deltaTime;

                if (attackTimer >= attack.attackDuration)
                {
                    attack.EndAttack(gameObject);
                    attackTimer = 0f;
                    waitingForNextAttack = true;
                    delayTimer = 0f;
                }
            }
            else
            {
                delayTimer += Time.deltaTime;
                if (delayTimer >= attack.delayAfter)
                {
                    currentAttackIndex = (currentAttackIndex + 1) % phase.attacks.Length;
                    phase.attacks[currentAttackIndex].StartAttack(gameObject);
                    waitingForNextAttack = false;
                }
            }

            // Phase switch check
            if (bossHealth.health <= phase.minHealthThreshold && currentPhaseIndex < phases.Length - 1)
            {
                currentPhaseIndex++;
                currentAttackIndex = 0;
                attackTimer = 0f;
                waitingForNextAttack = false;

                phase = phases[currentPhaseIndex];
                if (phase.attacks.Length > 0)
                    phase.attacks[0].StartAttack(gameObject);
            }
        }

        /// <summary>
        /// Get the position of a named point. Returns boss center if not found.
        /// </summary>
        public Vector3 GetPointPosition(string pointName)
        {
            foreach (var np in namedPoints)
            {
                if (np.name == pointName && np.transform != null)
                    return np.transform.position;
            }
            return transform.position; // fallback
        }

        /// <summary>
        /// Returns the Transform of a named point, or null if not found
        /// </summary>
        public Transform GetPointTransform(string pointName)
        {
            foreach (var np in namedPoints)
            {
                if (np.name == pointName && np.transform != null)
                    return np.transform;
            }

            return null; // fallback if not found
        }
    }
}