using Cards.Environments;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorCombatListener
    {
        public void StartMatch(RTCombatEnv env);
        public void EndMatch(RTCombatEnv env);
    }
}