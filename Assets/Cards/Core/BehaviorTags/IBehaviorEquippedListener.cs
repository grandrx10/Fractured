namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorEquippedListener
    {
        public void Equip(Card card, PlayerAgent agent);
        
        public void Unequip(Card card, PlayerAgent agent);
    }
}