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
        if (true)
        {
            _stacks++;
            if (_stacks >= stacks)
            {
                _stacks = 0;
                //rtc.Heal(healing);
            }
            SetStacks(env);
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
        _stacks = 0;
        SetStacks(env);
    }

    private void SetStacks(CardEnv env)
    {
        if (env.TryGetEffectById(out HealEffect effect, abilityName))
        {
            if (_stacks > 0) effect.SetOrbs(_stacks);
            else Destroy(effect);
        } else if (_stacks > 0)
        {
            var eff = env.AddEffect<HealEffect>(abilityName);
            eff.orbs = _stacks;
        }
    }

    public void EndMatch(Card card, RTCombatEnv env)
    {
        
    }
}
