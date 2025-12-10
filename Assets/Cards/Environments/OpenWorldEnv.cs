using System;
using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.PhysicalProperties;
using UnityEngine;

namespace Cards.Environments
{
    public class OpenWorldEnv: CardEnv
    {
        public Agent player;
        private PlayerInteractController _playerInteract;
        public PhysicalCard cardPrefab;
        public float mana, maxMana;
        
        public float manaRegen;
        public void Awake()
        {
            player.SelectCardAsync(UseCard, -1);
            _playerInteract = FindAnyObjectByType<PlayerInteractController>();
        }

        private void Update()
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
                return CardSubmitState.Success;
            }
            
            return CardSubmitState.Failure;
        }

        public void ThrowCard(Card card, Quaternion rotation, float speed)
        {
            var p = player.transform.position;
            var pLook = rotation * player.transform.forward;
            var d = rotation * (_playerInteract.GetCameraRaycastTarget() - p).normalized;
            var c = Instantiate(cardPrefab, p, Quaternion.LookRotation(d));
            c.card = card;
            c.InitState = new PhysicalCardObject.PhysicalCardInitState()
            {
                CenterPosition = p,
                StartDirection = pLook,
                StartPosition = p + pLook,
                TargetDirection = d,
                Speed = speed,
                Target = player.transform,
            };
            //c.GetComponent<Rigidbody>().linearVelocity = d;
        }
    }
}