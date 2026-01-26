using Cards.Environments;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Health
{
    public abstract class PlayerEffect : MonoBehaviour
    {
        public CardEnv env;
        public bool active = true;
        public string id;
        public abstract EffectStackBehavior Unique { get; }
        public abstract void TickEffect(float dt);

        public enum EffectStackBehavior
        {
            Multi,
            Unique,
            Replace,
            UniqueId,
        }
    }
}