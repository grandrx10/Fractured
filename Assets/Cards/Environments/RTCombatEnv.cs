using System;
using System.Collections.Generic;
using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cards.Environments
{
    public class RTCombatEnv: OpenWorldEnv
    {
        public float health;
        public float maxHealth;
        
        private void Start()
        {
            player.GetCards().ForEach(c =>
            {
                c.GetAllBehaviors<IBehaviorCombatListener>().ForEach(h => h.StartMatch());
            });
            health = player.TotalHealth;
            maxHealth = health;
            player.SelectCardAsync(UseCard, -1);
        }

        public bool TakeDamage(float damage)
        {
            health -= damage;
            health = Mathf.Clamp(health, 0, maxHealth);
            return true;
        }
        
        protected override void Update()
        {
            base.Update();
        }

        public void Terminate()
        {
            player.CancelSelection();
            player.GetCards().ForEach(c =>
            {
                c.GetAllBehaviors<IBehaviorCombatListener>().ForEach(h => h.EndMatch());
            });
            Debug.Log("Done");
        }
    }
}