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
        env.AddEffect<JuicedEffect>();

        return true;
    }
}
