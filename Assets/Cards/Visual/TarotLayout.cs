using Cards.Core;
using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Visual
{
    public class TarotLayout: CardInteractionContainer
    {
        public override void AddCardDisplay(Card card, int position=0)
        {
            if (!card.IsTarot) return;
            base.AddCardDisplay(card, position);
        }

        public override bool OnCardDropped(CardInteractionContainer source, CardDisplayInteractable card)
        {
            if (!card.AttachedCard.IsTarot) return false;
            return base.OnCardDropped(source, card);
        }
    }
}