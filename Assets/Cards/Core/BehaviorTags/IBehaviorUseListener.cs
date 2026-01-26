using Cards.Environments;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorUseListener
    {
        public bool Use(Card card, CardEnv env, Agent agent);
    }
}