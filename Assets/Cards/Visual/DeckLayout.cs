using System;
using UnityEngine;
using System.Collections.Generic;
using Cards.Core;
using Cards.Core.Behaviors;
using TMPro;
using UnityEngine.Serialization;

namespace Cards.Visual
{
    public class DeckLayout : CardInteractionContainer
    {
        public RectTransform content;
        public RectTransform infoCard;
        public CardPreview preview;
        public TextMeshProUGUI infoCardName;
        public TextMeshProUGUI infoCardColl;
        
        public CardDisplay cardPrefab;
        private CardDisplayInteractable _selectedCard;
        private CardPreview _currentPreview;
        public override void PopulateCards(List<Card> c)
        {
            for (int i = 0; i < c.Count; i++)
            {
                var cc = Instantiate(cardPrefab, content);
                cc.card = c[i];
                var hcc = cc.gameObject.AddComponent<CardDisplayInteractable>();
                hcc.Init(this);
                cards.Add(hcc);
            }
            //RefreshLayout();
        }

        public override void OnCardClick(CardDisplayInteractable card)
        {
            _selectedCard = card;
            RefreshLayout();
        }

        public override void RefreshLayout()
        {
            ValidateSelectedCard();
            foreach (Transform child in infoCard) Destroy(child.gameObject);
            
            if (_selectedCard)
            {
                var v = _selectedCard.AttachedCard.Visuals;
                infoCardName.text = v.Name;
                infoCardColl.text = v.CollectionName;
                
                var cc = Instantiate(cardPrefab, content);
                cc.transform.localScale *= 1.3f;
                cc.interactable = true;
                cc.card = _selectedCard.AttachedCard;
                cc.transform.SetParent(infoCard, false);
                cc.DisplayClicked += () =>
                {
                    CreatePreview(_selectedCard.AttachedCard);
                };
                foreach (var behavior in _selectedCard.AttachedCard.GetAllBehaviors<Behavior>())
                {
                    var desc = behavior.GetMenuObject();
                    if (desc != null)
                    {
                        desc.transform.SetParent(infoCard);
                    }
                }
            }
            else
            {
                infoCardName.text = "";
                infoCardColl.text = "";
            }
        }

        private void OnDisable()
        {
            if (_currentPreview) Destroy(_currentPreview.gameObject);
        }

        private void CreatePreview(Card c)
        {
            _currentPreview = Instantiate(preview);
            _currentPreview.transform.SetParent(transform.parent, true);
            _currentPreview.cardDisplay.card = c;
            _currentPreview.cardDisplay.interactable = true;
        }

        private void ValidateSelectedCard()
        {
            //if (_selectedCard && !cards.Contains(_selectedCard)) _selectedCard = null;
        }

        public override void AddCard(Card card)
        {
            var cc = Instantiate(cardPrefab, content);
            cc.card = card;
            var hcc = cc.gameObject.AddComponent<CardDisplayInteractable>();
            hcc.Init(this);
            cards.Add(hcc);
        }
    }
}