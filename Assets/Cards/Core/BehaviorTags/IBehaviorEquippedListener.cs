namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorEquippedListener
    {
        public void Equip(PlayerAgent agent);
        
        public void Unequip(PlayerAgent agent);
    }
}