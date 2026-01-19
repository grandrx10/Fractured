using System;
using System.Collections.Generic;
using System.Linq;
using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Characters;
using Game.Health;
using UnityEngine;
using UnityEngine.Serialization;
using World.Domain;
using Cards.Environments;
using Cards.Visual;
using Game;
using Characters.Dialogue;

namespace Cards.Environments
{
    public class RTCombatEnv: OpenWorldEnv
    {
        private float _health;
        public int shield;
        public int maxHealth = -1;
        public bool allowHeal = true;
        public bool allowShield = true;
        [SerializeField] private DiscreteContainer healthDisplay;
        [SerializeField] private DiscreteContainer shieldDisplay;
        public int damageIframes = 30;
        public DomainTrigger onDeath;
        private int _iframes;
        private PlayerHealth _healthInstance;
        public List<CardData> hope;
        public bool invincible = false;
        private bool _healthInitialized = false;

        public override void Initialize(PlayerAgent playerAgent)
        {
            base.Initialize(playerAgent);
            if (player.GetCards().Count == 0)
            {
                foreach (var h in hope)
                {
                    var c = new GameObject("hope").AddComponent<Card>();
                    c.AssignData(h);
                    player.GiveCard(c, true);
                }
            }
            
            playerAgent.GetCards().ForEach(c =>
            {
                c.GetAllBehaviors<IBehaviorCombatListener>().ForEach(h => h.StartMatch(c, this));
            });
            if (maxHealth == -1) maxHealth = playerAgent.TotalHealth + CurrentStats.health;
            maxHealth = Mathf.Max(maxHealth, 1);
            _health = maxHealth;
            _healthInstance = player.gameObject.AddComponent<PlayerHealth>();
            GameObject.FindAnyObjectByType<PlayerInventory>().MenuOff();
            _healthInstance.Init(this);
            PlayerInteractController.PlayerInputs.InCombat = true;
            _healthInitialized = true;
            UpdateHealth();
        }

        public void AddShield(int s)
        {
            if (!allowShield) return;
            shield += s;
            UpdateHealth();
        }

        public bool Heal(int amount)
        {
            if (!allowHeal || _health >= maxHealth) return false;
            _health += amount;
            _health = Mathf.Clamp(_health, 0, maxHealth);
            UpdateHealth();
            return true;
        }

        public bool TakeDamage(PlayerDamageData damage)
        {
            if (invincible)
                return false;

            if (_iframes > 0)
                return false;

            var listeners = player.GetCards()
                .SelectMany(card =>
                    card.GetAllBehaviors<IBehaviorTakeDamageListener>()
                        .Select(listener => (card, listener))
                )
                .OrderByDescending(x => x.listener.Priority)
                .ToList();

            foreach (var (card, listener) in listeners)
            {
                damage = listener.Hit(card, this, player, damage);
            }

            int dmg = Mathf.RoundToInt(Mathf.Max(damage.Damage, 0));
            
            int shieldDamage = Mathf.Min(dmg, shield);
            shield -= shieldDamage;
            dmg -= shieldDamage;
            _health -= dmg;
            _health = Mathf.Clamp(_health, 0, maxHealth);
            UpdateHealth();

            if (damage.Damage > 0 || damage.ForceIframes)
            {
                _iframes = damageIframes;
                return true;
            }

            return false;
        }


        private void UpdateHealth()
        {
            if (!initialized || !_healthInitialized) return;
            shield = Mathf.Min(shield, maxHealth);
            healthDisplay.SetMaxValue(maxHealth);
            shieldDisplay.SetMaxValue(maxHealth);
            healthDisplay.SetValue(_health);
            shieldDisplay.SetValue(shield);
            if (_health <= 0)
            {
                Die();
            }
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

        public void Die()
        {
            initialized = false;

            // ----------------------------
            // Increment death count
            // ----------------------------
            int deaths = 0;
            if (GlobalState.instance != null)
            {
                GlobalState.instance.TryGetInt("DeathCount", out deaths);
                deaths++;
                GlobalState.instance.SetInt("DeathCount", deaths);
            }

            // ----------------------------
            // Trigger death dialogue
            // ----------------------------
            if (DialogueManager.Instance != null)
            {
                string convoName = $"Death{deaths}";

                DialogueManager.Instance.StartConversation(convoName);
            }

            // ----------------------------
            // Trigger domain death logic (optional, AFTER dialogue)
            // ----------------------------
            onDeath.Trigger(player.transform.position - Vector3.up * 1.5f);
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
                c.GetAllBehaviors<IBehaviorCombatListener>().ForEach(h => h.EndMatch(c, this));
            });
            Destroy(_healthInstance);
            PlayerInteractController.PlayerInputs.InCombat = false;
            Debug.Log("Done");
        }
    }
}