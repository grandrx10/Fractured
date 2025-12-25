using System;
using System.Collections.Generic;
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
        public PlayerHealth healthInstance;
        
        public override void Initialize(PlayerAgent playerAgent)
        {
            playerAgent.GetCards().ForEach(c =>
            {
                c.GetAllBehaviors<IBehaviorCombatListener>().ForEach(h => h.StartMatch());
            });
            health = playerAgent.TotalHealth;
            maxHealth = health;
            healthInstance = player.gameObject.AddComponent<PlayerHealth>();
            base.Initialize(playerAgent);
        }

        public bool TakeDamage(float damage)
        {
            health -= damage;
            health = Mathf.Clamp(health, 0, maxHealth);
            return true;
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
            Destroy(healthInstance);
            Debug.Log("Done");
        }
    }
}