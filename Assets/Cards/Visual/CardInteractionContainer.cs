using System.Collections.Generic;
using Cards.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cards.Visual
{
    public class CardInteractionContainer : MonoBehaviour
    {
        protected List<CardDisplayInteractable> CardDisplays = new();
        protected List<Card> Cards;
        public RectTransform content;
        public CardDisplay cardPrefab;
        
        public void AssignCardList(List<Card> cards)
        {
            Cards = cards;
            PopulateCards();
        }
        
        public virtual void PopulateCards()
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                var cc = Instantiate(cardPrefab, content);
                cc.card = Cards[i];
                var hcc = cc.gameObject.AddComponent<CardDisplayInteractable>();
                hcc.Init(this);
                CardDisplays.Add(hcc);
            }
            RefreshLayout();
        }

        public virtual void AddCard(Card card, int position=0)
        {
            card.transform.SetParent(transform);
            var cc = Instantiate(cardPrefab, content);
            cc.card = card;
            if (!Cards.Contains(card)) Cards.Add(card);
            var hcc = cc.gameObject.AddComponent<CardDisplayInteractable>();
            hcc.Init(this);
            cc.transform.SetSiblingIndex(position);
            CardDisplays.Insert(position, hcc);
        }
        
        public virtual void RemoveCard(Card card)
        {
            if (Cards.Contains(card)) Cards.Remove(card);
            var c = CardDisplays.Find(c => c.AttachedCard == card);
            if (c != null) CardDisplays.Remove(c);
            RefreshLayout();
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
            CardDisplays.Remove(card);
            Cards.Remove(card.AttachedCard);
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