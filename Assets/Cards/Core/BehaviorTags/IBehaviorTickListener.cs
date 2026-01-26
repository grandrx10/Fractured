using Cards.Environments;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorTickListener
    {
        public void Tick(Card card, CardEnv env, Agent agent);
    }
}