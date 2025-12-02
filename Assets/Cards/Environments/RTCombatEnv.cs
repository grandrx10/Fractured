using System;
using System.Collections.Generic;
using Cards.Behaviors;
using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using UnityEngine;

namespace Cards.Environments
{
    public class RTCombatEnv: MonoBehaviour
    {
        public Agent playerAgent;
        public float currentMana;
        
        private void Start()
        {
            playerAgent.GetCards().ForEach(c =>
            {
                c.GetAllBehaviors<IBehaviorRTUpdateListener>().ForEach(h => h.EndMatch());
            });
            playerAgent.SelectCardAsync(UseCard, -1);
        }

        private CardSubmitState UseCard(Card card)
        {
            if (card.stats.mana < currentMana) return CardSubmitState.Failure;
            return CardSubmitState.Success;
        }
        
        private void Update()
        {
            //throw new NotImplementedException();
        }

        public void Terminate()
        {
            playerAgent.CancelSelection();
            playerAgent.GetCards().ForEach(c =>
            {
                c.GetAllBehaviors<IBehaviorRTUpdateListener>().ForEach(h => h.EndMatch());
            });
            Debug.Log("Done");
        }
    }
}