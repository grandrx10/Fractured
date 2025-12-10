using System;
using System.Collections.Generic;
using Cards.Behaviors;
using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cards.Environments
{
    public class RTCombatEnv: CardEnv
    {
        public Agent playerAgent;
        [FormerlySerializedAs("currentMana")] public float mana;
        
        private void Start()
        {
            playerAgent.GetCards().ForEach(c =>
            {
                c.GetAllBehaviors<IBehaviorCombatListener>().ForEach(h => h.StartMatch());
            });
            playerAgent.SelectCardAsync(UseCard, -1);
        }

        private CardSubmitState UseCard(Card card)
        {
            if (card.stats.mana < mana) return CardSubmitState.Failure;
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
                c.GetAllBehaviors<IBehaviorCombatListener>().ForEach(h => h.EndMatch());
            });
            Debug.Log("Done");
        }
    }
}