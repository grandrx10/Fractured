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
        [SerializeField] private List<Transform> environmentCenters;
        [HideInInspector] public bool initialized;
        
        [HideInInspector] public Agent player;
        protected PlayerStats CurrentStats;
        
        private List<PlayerEffect> _effects = new List<PlayerEffect>();
        
        private void Start()
        {
            GlobalWorldManager.Instance.Load(this);
        }

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
            _effects.RemoveAll(e => e is T);
        }
        
        public T AddEffect<T>() where T : PlayerEffect
        {
            if (_effects.Exists(e => e is T && e.Unique)) return null;
            
            var effect = gameObject.AddComponent<T>();
            _effects.Add(effect);
            return effect;
        }
        
        protected virtual void Update()
        {
            foreach (var effect in _effects)
            {
                effect.TickEffect(Time.deltaTime);
            }
        }

        public virtual void Initialize(PlayerAgent p)
        {
            initialized = true;
            player = p;
            p.OnStatsUpdate += () =>
            {
                CurrentStats = p.stats.GetStats();
            };
        }
        
        public virtual void Destroy()
        {
            foreach (var card in player.GetCards().
                         FindAll(c => c.TryGetBehavior(out TemporaryBehavior temp) && !temp.persistent))
            {
                player.RemoveCard(card);
                Destroy(card.gameObject);
            }
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