namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorCombatListener
    {
        public void StartMatch();
        public void EndMatch();
        public void TakeDamage(int value);
    }
}