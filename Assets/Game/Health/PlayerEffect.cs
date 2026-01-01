using Cards.Environments;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Health
{
    public abstract class PlayerEffect : MonoBehaviour
    {
        public CardEnv env;
        public bool active = true;
        public abstract bool Unique { get; }
        public abstract void TickEffect(float dt);
    }
}