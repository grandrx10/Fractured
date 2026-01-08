using Cards.Core;
using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Visual
{
    public class TarotLayout: CardInteractionContainer
    {
        public override void AddCard(Card card, bool force=true, int position=0)
        {
            if (!card.IsTarot) return;
            base.AddCard(card, force, position);
        }

        public override bool OnCardDropped(CardInteractionContainer source, CardDisplayInteractable card)
        {
            if (!card.AttachedCard.IsTarot) return false;
            return base.OnCardDropped(source, card);
        }
    }
}