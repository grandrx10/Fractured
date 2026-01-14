using Cards.Core;
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
        public bool showDesc = true;
        public virtual bool Use(Card card, CardEnv env, Agent agent)
        {
            var cardPrev = Instantiate(preview, GameObject.FindGameObjectWithTag("Main UI").transform);
            cardPrev.cardDisplay.card = card;
            cardPrev.cardDisplay.interactable = true;
            cardPrev.cardDisplay.hasDepth = true;
            return true;
        }

        public override string GetDescription(Card card)
        {
            return showDesc ? "<b>Cannot be thrown.</b>" : "";
        }
    }
}