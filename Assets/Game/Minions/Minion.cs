using System.Collections.Generic;
using Cards.Environments;
using UnityEngine;

namespace Game.Minions
{
    public class Minion : MonoBehaviour
    {
        [Header("Health")]
        public Game.Health.Health health;

        [Header("Attacks")]
        public MinionAttack[] attacks;

        private int currentAttackIndex;
        private MinionAttack currentAttack;

        private float attackTimer;
        private float delayTimer;
        private bool waitingForNextAttack;

        private bool _initialized;

        private void Awake()
        {
            // Wait for world to initialize
            GlobalWorldManager.OnLoadNewScene += Init;
        }

        private void Init(CardEnv environment)
        {
            GlobalWorldManager.OnLoadNewScene -= Init;

            if (attacks == null || attacks.Length == 0)
                return;

            // Initialize all attacks with this minion
            foreach (var attack in attacks)
                attack.Initialize(this);

            _initialized = true;
            currentAttackIndex = 0;
            waitingForNextAttack = false;
            StartNextAttack();
        }

        private void Update()
        {
            if (!_initialized || attacks == null || attacks.Length == 0)
                return;

            if (!waitingForNextAttack)
            {
                attackTimer += Time.deltaTime;
                currentAttack.Tick();

                if (attackTimer >= currentAttack.duration)
                    EndCurrentAttack();
            }
            else
            {
                delayTimer += Time.deltaTime;
                if (delayTimer >= currentAttack.delayAfter)
                    StartNextAttack();
            }
        }

        private void StartNextAttack()
        {
            delayTimer = 0f;
            waitingForNextAttack = false;

            currentAttackIndex = currentAttackIndex % attacks.Length;
            currentAttack = attacks[currentAttackIndex];

            attackTimer = 0f;
            currentAttack.Activate();
        }

        private void EndCurrentAttack()
        {
            currentAttack.End();

            attackTimer = 0f;
            delayTimer = 0f;
            waitingForNextAttack = true;

            currentAttackIndex = (currentAttackIndex + 1) % attacks.Length;
        }

        private void OnEnable()
        {
            if (_initialized)
            {
                currentAttackIndex = 0;
                waitingForNextAttack = false;
                StartNextAttack();
            }
        }
    }
}
