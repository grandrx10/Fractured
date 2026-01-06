using UnityEngine;
using Cards.Environments;

namespace Game.Minions
{
    public abstract class MinionAttack : MonoBehaviour
    {
        [Header("Timing")]
        public float duration = 2f;
        public float delayAfter = 1f;

        protected Minion owner;
        protected Transform minionTransform;
        protected Transform playerTransform;

        private bool isActive;

        public void Initialize(Minion minion)
        {
            owner = minion;
            minionTransform = minion.transform;
        }

        public virtual void Activate()
        {
            isActive = true;
            playerTransform = OpenWorldEnv.Current.PlayerTransform;
        }

        public virtual void Tick() { }

        public virtual void End()
        {
            isActive = false;
        }

        protected bool Active => isActive;
    }
}
