using Cards.Environments;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorUseListener
    {
        public bool Use(CardEnv env, Agent agent);
    }
}