using Cards.Core;
using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Visual
{
    public class TarotLayout: CardInteractionContainer
    {
        public override void AddCard(Card card, int position=0)
        {
            if (!card.IsTarot) return;
            base.AddCard(card, position);
        }
    }
}