using System;
using System.Collections.Generic;
using Cards.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cards.Visual
{
    public class CardInteractionContainer : MonoBehaviour
    {
        [SerializeField] protected List<CardDisplayInteractable> CardDisplays = new();
        protected List<Card> Cards;
        public RectTransform content;
        public CardDisplay cardPrefab;
        protected Action<Card> AddCard, RemoveCard;
        public void AssignCardList(List<Card> cards, Action<Card> addCard, Action<Card> removeCard)
        {
            Cards = cards;
            AddCard = addCard;
            RemoveCard = removeCard;
            PopulateCards();
        }
        
        public virtual void PopulateCards()
        {
            foreach (var t in Cards)
            {
                AddCardDisplay(t);
            }

            RefreshLayout();
        }

        public virtual void AddCardDisplay(Card card, int position=0)
        {
            if (!Cards.Contains(card)) return;
            
            card.transform.SetParent(transform);
            var cc = Instantiate(cardPrefab, content);
            cc.card = card;
            var hcc = cc.gameObject.AddComponent<CardDisplayInteractable>();
            hcc.Init(this);
            cc.transform.SetSiblingIndex(position);
            CardDisplays.Insert(position, hcc);
        }
        
        public virtual void RemoveCardDisplay(Card card)
        {
            CardDisplayInteractable c = null;
            foreach (var cc in CardDisplays)
            {
                //Debug.Log($"{cc.AttachedCard}, {card}");
                if (cc && ReferenceEquals(cc.AttachedCard, card))
                {
                    c = cc;
                }
            }
            if (c != null)
            {
                CardDisplays.Remove(c);
                Destroy(c.gameObject);
            }
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
                AddCardDisplay(card.AttachedCard);
                RefreshLayout();
                return true;
            }
            return false;
        }
        
        public virtual bool OnCardRemoved(CardDisplayInteractable card)
        {
            RemoveCard(card.AttachedCard);
            RemoveCardDisplay(card.AttachedCard);
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