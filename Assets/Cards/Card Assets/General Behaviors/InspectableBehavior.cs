using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Cards.Visual;
using UnityEngine;
using Utils;

namespace Cards.Card_Assets.General_Behaviors
{
    [CreateAssetMenu(fileName = "Inspectable", menuName = "Behaviors/Inspectable")]
    public class InspectableBehavior: Behavior, IBehaviorUseListener
    {
        public CardPreview preview;
        public virtual bool Use(CardEnv env, Agent agent)
        {
            var cardPrev = Instantiate(preview, GameObject.FindGameObjectWithTag("Main UI").transform);
            cardPrev.cardDisplay.card = AttachedCard;
            cardPrev.cardDisplay.interactable = true;
            cardPrev.cardDisplay.hasDepth = true;
            return true;
        }
    }
}