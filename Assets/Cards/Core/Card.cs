using System;
using System.Collections.Generic;
using System.Linq;
using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Core
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private CardData data;
        public CardVisuals Visuals => data.Visuals;
        public bool Active { get; private set; }
        public CardTier tier;
        public CardStats stats;
        public List<Behavior> behaviors;
        private bool _initialized;
        public void AssignData(CardData d)
        {
            data = d;
            Initialize();
        }
        private void Awake()
        {
            if (!data) return;
            Initialize();
        }

        private void Initialize()
        {
            tier = data.tier;
            stats = data.stats;
            behaviors = data.behaviors.ToList();
            
            CreateDefault<DefaultUseBehavior>();
            CreateDefault<DefaultCollideBehavior>();
            CreateDefault<DefaultCardGameBehavior>();
            CreateDefault<DefaultHealthBehavior>();
            
            for (int i = 0; i < behaviors.Count; i++)
            {
                behaviors[i] = Instantiate(behaviors[i]);
                behaviors[i].Init(this);
            }
            _initialized = true;
        }

        public void UpdateActive()
        {
            bool b = true;
            behaviors.ForEach(be =>
            {
                if (be.Active) b = false;
            });
            Active = b;
        }
        public bool TryGetBehavior<T>(out T behavior) where T : class
        {
            foreach (var b in behaviors)
            {
                if (b is T baseBehavior)
                {
                    behavior = baseBehavior;
                    return true;
                }
            }

            behavior = null;
            return false;
        }

        private void CreateDefault<T>() where T : Behavior
        {
            if (!TryGetBehavior(out T _)) behaviors.Add(ScriptableObject.CreateInstance<T>());
        }
        
        public List<T> GetAllBehaviors<T>() where T : class
        {
            List<T> targ = new List<T>();
            foreach (var b in behaviors)
            {
                if (b is T behavior)
                {
                    targ.Add(behavior);
                }
            }
            return targ;
        }
    }
}
