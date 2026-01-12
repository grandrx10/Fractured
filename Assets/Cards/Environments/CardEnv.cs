using System;
using System.Collections.Generic;
using System.Linq;
using Cards.Card_Assets.General_Behaviors;
using Characters.Player;
using Game.Health;
using UnityEngine;

namespace Cards.Environments
{
    public enum CardSubmitState
    {
        Invalid,
        Success,
        Failure
    }

    // We could probably replace this with some interfaces
    // Currently the expectation is that cards type check the CardEnv parameter to execute behavior
    public class CardEnv: MonoBehaviour
    {
        public float environmentExitTime;
        public float environmentIntroTime;
        public float environmentIntroRad;
        public RaviMode raviMode = RaviMode.Normal;
        [SerializeField] private List<Transform> environmentCenters;
        [HideInInspector] public bool initialized;
        
        [SerializeField] private DiscreteContainer manaDisplay;
        public float mana;
        
        [HideInInspector] public PlayerAgent player;
        protected PlayerStats CurrentStats;
        
        private List<PlayerEffect> _effects = new List<PlayerEffect>();
        public Action OnLoad;
        public Transform GetEnvCenter(string n)
        {
            var t = environmentCenters.Find(x => x.name == n);
            return t;
        }
        
        public bool TryGetEffect<T>(out T effect) where T : PlayerEffect
        {
            var e = _effects.Find(e => e is T);
            effect = e as T;
            return e != null;
        }
        
        public bool HasEffect<T>() where T : PlayerEffect
        {
            var e = _effects.Find(e => e is T);
            return e != null;
        }
        
        public void RemoveEffect<T>() where T : PlayerEffect
        {
            _effects.RemoveAll(e =>
            {
                if (e is T)
                {
                    Destroy(e);
                    return true;
                }
                
                return false;
            });
        }
        
        public T AddEffect<T>() where T : PlayerEffect
        {
            var other = _effects.Find(e => e is T);
            if (other)
            {
                if (other.Unique == PlayerEffect.EffectStackBehavior.Unique) return null;
                if (other.Unique == PlayerEffect.EffectStackBehavior.Replace) Destroy(other);
            }
            
            var effect = gameObject.AddComponent<T>();
            effect.env = this;
            _effects.Add(effect);
            return effect;
        }
        
        List<PlayerEffect> _deleted = new();
        
        protected virtual void Update()
        {
            foreach (var effect in _effects)
            {
                if (effect) effect.TickEffect(Time.deltaTime);
                else _deleted.Add(effect);
            }
            _deleted.ForEach(o =>
            {
                if (o) Destroy(o);
                _effects.Remove(o);
            });
            _deleted.Clear();
            manaDisplay.SetMaxValue(CurrentStats.maxMana);
            manaDisplay.SetValue(mana);
        }

        public virtual void Initialize(PlayerAgent p)
        {
            initialized = true;
            player = p;
            p.OnStatsUpdate += UpdateStats;
            OnLoad?.Invoke();
        }

        private void UpdateStats()
        {
            CurrentStats = player.stats.GetStats();
        }
        
        public virtual void Destroy()
        {
            foreach (var card in player.GetAllCards().
                         FindAll(c => c.TryGetBehavior(out TemporaryBehavior temp) && !temp.persistent))
            {
                Debug.Log($"lost {card.Visuals.Name}");
                player.TakeCard(card);
                Destroy(card.gameObject);
            }
            player.OnStatsUpdate -= UpdateStats;
        }

        private void OnDrawGizmos()
        {
            foreach (var v in environmentCenters)
            {
                Gizmos.DrawWireSphere(v.position, environmentIntroRad);
#if UNITY_EDITOR
                UnityEditor.Handles.Label(v.position, v.name);
                Gizmos.DrawSphere(v.position, 0.05f);
#endif
            }
        }
    }
}