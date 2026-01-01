using Cards.Environments;
using Game.Health;
using UnityEngine;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorTakeDamageListener
    {
        public float Priority { get; }
        public PlayerDamageData Hit(OpenWorldEnv env, Agent agent, PlayerDamageData data);
    }
}