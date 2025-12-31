using System;
using System.Collections.Generic;
using System.Linq;
using Cards.Core;
using Cards.Environments;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cards
{
    public class Agent: MonoBehaviour
    {
        public List<Card> hand;
        public List<Card> deck;
        public Card selectedCard;
        private Func<Card, CardSubmitState> _callback;
        private int _cardsRequested;
        public bool CardRequested => _callback != null;
        protected Card RandomCard => hand[Random.Range(0, hand.Count)];
        public Action<Card> OnAddCard;
        public Action<Card> OnUseCard;
        public int TotalHealth => hand.Sum(h => h.stats.health);
        
        /*
         * For selecting cards normally
         */
        public virtual void SelectCardAsync(Func<Card, CardSubmitState> callback, int requiredCards)
        {
            if (_callback != null) Debug.LogError("Already awaiting a card!");
            _callback = callback;
            _cardsRequested = requiredCards;
        }

        public virtual void CancelSelection()
        {
            _callback = null;
        }
        
        /*
         * Implicitly gets the top priority card
         */
        public virtual Card SelectCardImmediate()
        {
            return RandomCard;
        }

        public void AddCard(Card card)
        {
            deck.Add(card);
            OnAddCard?.Invoke(card);
            card.transform.parent = transform;
        }

        public List<Card> GetCards()
        {
            return hand;
        }

        public CardSubmitState SubmitCard(Card card)
        {
            if (_callback == null) return CardSubmitState.Invalid;
            var s = _callback.Invoke(card);
            
            if (s == CardSubmitState.Failure) return CardSubmitState.Failure;
            _cardsRequested--;
            if (_cardsRequested == 0) _callback = null;
            OnUseCard?.Invoke(card);
            return s;
        }
        
        [ContextMenu("pick random")]
        protected void PickRandom()
        {
            var card = RandomCard;
            Debug.Log($"{name} playing card: {card}");
            SubmitCard(card);
        }
    }
}