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
        public void Awake()
        {
            player.SelectCardAsync(UseCard, 1);
            _playerInteract = FindAnyObjectByType<PlayerInteractController>();
        }

        protected virtual void Update()
        {
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