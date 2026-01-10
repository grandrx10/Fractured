using System;
using System.Collections.Generic;
using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.PhysicalProperties;
using Characters;
using Characters.Player;
using Game.Health;
using UnityEngine;
using Utils;

namespace Cards.Environments
{
    public class OpenWorldEnv: CardEnv
    {
        public static OpenWorldEnv Current => GlobalWorldManager.Instance.CurrentEnvironment as OpenWorldEnv;
        private PlayerInteractController _playerInteract;
        
        public float minCardDelay = 0.2f;
        protected PlayerMovement PlayerMovement;
        
        public override void Initialize(PlayerAgent playerAgent)
        {
            player = playerAgent;
            playerAgent.SelectCardAsync(UseCard, 1);
            _playerInteract = FindAnyObjectByType<PlayerInteractController>();
            base.Initialize(playerAgent);
            
            PlayerMovement = playerAgent.GetComponentInParent<PlayerMovement>();
            playerAgent.OnStatsUpdate += () =>
            {
                PlayerMovement.moveSpeed = CurrentStats.speed;
                PlayerMovement.jumpForce = CurrentStats.jumpPower;
            };
            playerAgent.UpdateStats();
        }
        
        public Vector3 GetBossTargetGrounded(float distance = 10f)
        {
            return PlayerMovement.GetPositionBelow(distance);
        }

        public override void Destroy()
        {
            StopAllCoroutines();
            player.CancelSelection();
            base.Destroy();
        }

        protected override void Update()
        {
            if (!initialized) return;
            mana += CurrentStats.manaRegen * Time.deltaTime * (mana/CurrentStats.maxMana/2 + 1f);
            mana = Mathf.Clamp(mana, 0, CurrentStats.maxMana);
           
            var ticks = player.selectedCard?.GetAllBehaviors<IBehaviorTickListener>();
            if (ticks != null)
            {
                foreach (var t in ticks)
                {
                    t.Tick(this, player);
                }
            }
            base.Update();
        }

        public CardSubmitState UseCard(Card card)
        {
            if (card.stats.mana <= mana)
            {
                bool used = false;
                foreach (var useBehavior in card.GetAllBehaviors<IBehaviorUseListener>())
                {
                    used = useBehavior.Use(this, player) || used;
                }

                if (used)
                {
                    mana -= card.stats.mana;
                    Delay.Call(this, minCardDelay, () =>
                    {
                        player.SelectCardAsync(UseCard, 1);
                    });
                }
                
                
                return used ? CardSubmitState.Success : CardSubmitState.Failure;
            }
            
            return CardSubmitState.Failure;
        }
        
        public Transform PlayerTransform => player.transform;
        public Vector3 PlayerPos => player.transform.position;
        
        public Vector3 PlayerLook => (_playerInteract.GetCameraRaycastTarget() - player.transform.position).normalized;

        public RaycastHit GetPlayerLookTarget(LayerMask layers)
        {
            return _playerInteract.GetPlayerLookTarget(layers);
        }
    }
}