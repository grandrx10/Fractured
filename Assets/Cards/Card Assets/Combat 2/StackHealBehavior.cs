using Cards;
using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Game.Effects;
using UnityEngine;

[CreateAssetMenu(menuName = "Behaviors/StackHeal")]
public class StackHealBehavior : Behavior, IBehaviorUseListener, IBehaviorHasStateTag, IBehaviorCombatListener
{
    public int stacks;
    public int healing;
    private int _stacks;
    public string abilityName;
    public bool Use(Card card, CardEnv env, Agent agent)
    {
        if (env is RTCombatEnv rtc)
        {
            _stacks++;
            if (_stacks >= stacks)
            {
                stacks = 0;
                rtc.Heal(healing);
                SetStacks(env);
            }
            return true;
        }
        return false;
    }

    public override string GetDescription(Card card)
    {
        return $"<b>(Active) {abilityName}:</b> Gain a stack. At {stacks} stacks, heal {healing} health.";
    }

    public Card AttachedCard { get; set; }
    public void StartMatch(Card card, RTCombatEnv env)
    {
        stacks = 0;
        SetStacks(env);
    }

    private void SetStacks(CardEnv env)
    {
        if (env.TryGetEffectById(out HealEffect effect, abilityName))
        {
            if (stacks > 0) effect.SetOrbs(0);
            else Destroy(effect);
        } else if (stacks > 0)
        {
            var eff = env.AddEffect<HealEffect>(abilityName);
            eff.SetOrbs(stacks);
        }
    }

    public void EndMatch(Card card, RTCombatEnv env)
    {
        throw new System.NotImplementedException();
    }
}
