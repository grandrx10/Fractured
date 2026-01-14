using UnityEngine;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Characters;
using Cards;
using Cards.Core;
using Game.Effects;

[CreateAssetMenu(menuName = "Behaviors/JuicerBehavior")]
public class JuicerBehavior : Behavior, IBehaviorUseListener
{
    public bool Use(Card card, CardEnv env, Agent agent)
    {
        return env.AddEffect<JuicedEffect>();
    }

    public override string GetDescription(Card card)
    {
        return "<b>(Active) Juicer:</b> Empowers the next fruit used.";
    }
}
