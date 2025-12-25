using Cards.Environments;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorTickListener
    {
        public void Tick(CardEnv env, Agent agent);
    }
}