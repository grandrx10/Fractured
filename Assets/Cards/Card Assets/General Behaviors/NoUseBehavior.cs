using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;
using Utils;

namespace Cards.Card_Assets.General_Behaviors
{
    [CreateAssetMenu(fileName = "NoUse", menuName = "Behaviors/NoUse")]
    public class NoUseBehavior: Behavior, IBehaviorUseListener
    {
        public bool yap;
        public override string GetDescription(Card card)
        {
            return yap ? $"<b>Can't be thrown</b>" : "";
        }

        public bool Use(Card card, CardEnv env, Agent agent)
        {
            return false;
        }
    }
}