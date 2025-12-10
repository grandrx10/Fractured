using Cards.Environments;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorUseListener
    {
        public void Use(CardEnv env, Agent agent);
    }
}