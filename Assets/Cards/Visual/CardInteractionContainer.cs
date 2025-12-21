using System.Collections.Generic;
using Cards.Core;
using UnityEngine;

namespace Cards.Visual
{
    public class CardInteractionContainer : MonoBehaviour
    {
        public List<CardDisplayInteractable> cards;

        public virtual void PopulateCards(List<Card> c)
        {
            throw new System.NotImplementedException();
        }

        public virtual void AddCard(Card card, int position=0)
        {
            throw new System.NotImplementedException();
        }

        public virtual void RefreshLayout()
        {
            
        }
        
        public virtual bool OnCardDropped(CardInteractionContainer source, CardDisplayInteractable card)
        {
            if (source == this) return false;
            if (source.OnCardRemoved(card))
            {
                AddCard(card.AttachedCard);
                Destroy(card.gameObject);
                RefreshLayout();
                return true;
            }
            return false;
        }
        
        public virtual bool OnCardRemoved(CardDisplayInteractable card)
        {
            cards.Remove(card);
            RefreshLayout();
            return true;
        }
        public virtual void OnCardClick(CardDisplayInteractable card)
        {

        }
        public virtual void OnCardStopHover(CardDisplayInteractable card)
        {

        }
        public virtual void OnCardStartHover(CardDisplayInteractable card)
        {

        }
    }
}