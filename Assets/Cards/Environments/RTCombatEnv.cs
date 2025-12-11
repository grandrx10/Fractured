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
        public int health;
        
        private void Start()
        {
            player.GetCards().ForEach(c =>
            {
                c.GetAllBehaviors<IBehaviorCombatListener>().ForEach(h => h.StartMatch());
            });
            health = player.TotalHealth;
            player.SelectCardAsync(UseCard, -1);
        }
        
        private void Update()
        {
            //throw new NotImplementedException();
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