using System;
using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.PhysicalProperties;
using Characters;
using UnityEngine;
using Utils;

namespace Cards.Environments
{
    public class OpenWorldEnv: CardEnv
    {
        public Agent player;
        private PlayerInteractController _playerInteract;
        public float mana, maxMana;
        public float minCardDelay = 0.2f;
        
        public float manaRegen;
        public bool initialized;
        
        public override void Initialize(PlayerAgent playerAgent)
        {
            player = playerAgent;
            playerAgent.SelectCardAsync(UseCard, 1);
            _playerInteract = FindAnyObjectByType<PlayerInteractController>();
            initialized = true;
        }

        public override void Destroy()
        {
            player.CancelSelection();
        }

        protected virtual void Update()
        {
            if (!initialized) return;
            mana += manaRegen * Time.deltaTime;
            mana = Mathf.Clamp(mana, 0, maxMana);
        }

        public CardSubmitState UseCard(Card card)
        {
            if (card.stats.mana <= mana)
            {
                mana -= card.stats.mana;
                foreach (var useBehavior in card.GetAllBehaviors<IBehaviorUseListener>())
                {
                    useBehavior.Use(this, player);
                }
                Delay.Call(this, minCardDelay, () =>
                {
                    player.SelectCardAsync(UseCard, 1);
                });
                
                return CardSubmitState.Success;
            }
            
            return CardSubmitState.Failure;
        }

        public Vector3 GetPlayerLook()
        {
            return (_playerInteract.GetCameraRaycastTarget() - player.transform.position).normalized;
        }
    }
}