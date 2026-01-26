using Cards.Environments;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorCombatListener
    {
        public void StartMatch(Card card, RTCombatEnv env);
        public void EndMatch(Card card, RTCombatEnv env);
    }
}