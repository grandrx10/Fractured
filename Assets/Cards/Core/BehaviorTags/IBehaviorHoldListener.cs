namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorHoldListener
    {
        public void StartHold(Card card);
        public void StopHold(Card card);
    }
}