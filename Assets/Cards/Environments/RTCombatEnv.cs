using System;
using System.Collections.Generic;
using System.Linq;
using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Characters;
using Game.Health;
using UnityEngine;
using UnityEngine.Serialization;
using World.Domain;

namespace Cards.Environments
{
    public class RTCombatEnv: OpenWorldEnv
    {
        private float _health;
        public int maxHealth = -1;
        public bool allowHeal = true;
        [SerializeField] private DiscreteContainer healthDisplay;
        
        public int damageIframes = 30;
        public DomainTrigger onDeath;
        private int _iframes;
        private PlayerHealth _healthInstance;
        
        public override void Initialize(PlayerAgent playerAgent)
        {
            base.Initialize(playerAgent);
            
            playerAgent.GetCards().ForEach(c =>
            {
                c.GetAllBehaviors<IBehaviorCombatListener>().ForEach(h => h.StartMatch());
            });
            if (maxHealth == -1) maxHealth = playerAgent.TotalHealth + CurrentStats.health;
            maxHealth = Mathf.Max(maxHealth, 1);
            _health = maxHealth;
            _healthInstance = player.gameObject.AddComponent<PlayerHealth>();
            _healthInstance.Init(this);
            PlayerInteractController.PlayerInputs.InCombat = true;
            UpdateHealth();
        }

        public bool Heal(int amount)
        {
            if (!allowHeal || _health >= maxHealth) return false;
            _health += amount;
            _health = Mathf.Clamp(_health, 0, maxHealth);
            UpdateHealth();
            return true;
        }

        public bool TakeDamage(PlayerDamageData damage)
        {
            if (_iframes > 0) return false;
            
            var listeners = player.GetCards()
                .SelectMany(c => c.GetAllBehaviors<IBehaviorTakeDamageListener>())
                .OrderByDescending(b => b.Priority)
                .ToList();
            foreach (var l in listeners)
            {
                damage = l.Hit(this, player, damage);
            }
            _health -= Mathf.Max(damage.Damage, 0);
            _health = Mathf.Clamp(_health, 0, maxHealth);
            UpdateHealth();
            if (damage.Damage > 0 || damage.ForceIframes)
            {
                _iframes = damageIframes;
                return true;
            }
            return false;
        }

        private void UpdateHealth()
        {
            if (!initialized) return;
            healthDisplay.SetMaxValue(maxHealth);
            healthDisplay.SetValue(_health);
            if (_health <= 0)
            {
                Die();
            }
        }

        private void FixedUpdate()
        {
            _iframes--;
        }

        protected override void Update()
        {
            if (!initialized) return;
            base.Update();
        }

        public void Die()
        {
            initialized = false;
            onDeath.Trigger(player.transform.position - Vector3.up * 1.5f);
        }

        public override void Destroy()
        {
            Terminate();
            base.Destroy();
        }

        public void Terminate()
        {
            player.CancelSelection();
            player.GetCards().ForEach(c =>
            {
                c.GetAllBehaviors<IBehaviorCombatListener>().ForEach(h => h.EndMatch());
            });
            Destroy(_healthInstance);
            PlayerInteractController.PlayerInputs.InCombat = false;
            Debug.Log("Done");
        }
    }
}