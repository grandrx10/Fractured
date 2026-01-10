using UnityEngine;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Characters;
using Cards;
using Game.Effects;

[CreateAssetMenu(menuName = "Behaviors/JuicerBehavior")]
public class JuicerBehavior : Behavior, IBehaviorUseListener
{
    public bool Use(CardEnv env, Agent agent)
    {
        return env.AddEffect<JuicedEffect>();
    }

    public override string GetDescription()
    {
        return "<b>Juicer:</b> On Use, empowers the next fruit you use.";
    }
}
