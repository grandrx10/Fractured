using System;
using System.Collections.Generic;
using System.Linq;
using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Game.Health;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cards.Environments
{
    public class RTCombatEnv: OpenWorldEnv
    {
        public float health;
        public float maxHealth;
        public int damageIframes = 30;
        private int _iframes;
        private PlayerHealth _healthInstance;
        
        public override void Initialize(PlayerAgent playerAgent)
        {
            base.Initialize(playerAgent);
            
            playerAgent.GetCards().ForEach(c =>
            {
                c.GetAllBehaviors<IBehaviorCombatListener>().ForEach(h => h.StartMatch());
            });
            health = playerAgent.TotalHealth + CurrentStats.health;
            maxHealth = health;
            _healthInstance = player.gameObject.AddComponent<PlayerHealth>();
            _healthInstance.Init(this);
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
            if (damage.Damage > 0 || damage.ForceIframes)
            {
                _iframes = damageIframes;
                return true;
            }
            return false;
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
            Debug.Log("Done");
        }
    }
}