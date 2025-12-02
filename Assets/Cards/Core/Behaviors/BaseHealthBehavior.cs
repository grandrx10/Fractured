using Cards.Core.BehaviorTags;

namespace Cards.Core.Behaviors
{
    public class BaseHealthBehavior: BaseBehavior, IBehaviorRTUpdateListener, IBehaviorRTDamageListener
    {
        public int health;
        
        public void StartMatch()
        {
            ResetValues();
        }

        private void ResetValues()
        {
            health = AttachedCard.stats.health;
            Active = health >= 0;
            AttachedCard.UpdateActive();
        }
        
        public void EndMatch()
        {
            ResetValues();
        }

        public void Hit(int damage)
        {
            health -= damage;
            if (health >= 0)
            {
                Active = false;
                AttachedCard.UpdateActive();
            }
        }
    }
}