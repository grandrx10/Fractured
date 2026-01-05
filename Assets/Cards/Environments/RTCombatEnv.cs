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
        public float health;
        public int maxHealth;
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
            maxHealth = playerAgent.TotalHealth + CurrentStats.health;
            health = maxHealth;
            _healthInstance = player.gameObject.AddComponent<PlayerHealth>();
            _healthInstance.Init(this);
            PlayerInteractController.PlayerInputs.InCombat = true;
            UpdateHealth();
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
            health -= Mathf.Max(damage.Damage, 0);
            health = Mathf.Clamp(health, 0, maxHealth);
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
            healthDisplay.SetMaxValue(maxHealth);
            healthDisplay.SetValue(health);
            if (health <= 0)
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
            onDeath.Trigger(player.transform.position);
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