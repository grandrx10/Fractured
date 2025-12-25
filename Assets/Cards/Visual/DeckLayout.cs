using System;
using UnityEngine;
using System.Collections.Generic;
using Cards.Core;
using Cards.Core.Behaviors;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

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
            _currentPreview = Instantiate(preview, UIHelper.GetRootCanvas(transform).transform);
            _currentPreview.cardDisplay.card = c;
            _currentPreview.cardDisplay.interactable = true;
            _currentPreview.cardDisplay.hasDepth = true;
        }

        private void ValidateSelectedCard()
        {
            //if (_selectedCard && !cards.Contains(_selectedCard)) _selectedCard = null;
        }
        
        public override bool OnCardDropped(CardInteractionContainer source, CardDisplayInteractable card)
        {
            if (source.OnCardRemoved(card))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    content,
                    Input.mousePosition,
                    UIHelper.UICamera,
                    out var pointerLocal
                );
                    
                int targetIndex = GetGridInsertIndexLocal(
                    content.GetComponent<GridLayoutGroup>(),
                    content, 
                    pointerLocal,
                    cards.Count);
                
                AddCard(card.AttachedCard, targetIndex);
                
                Destroy(card.gameObject);
                RefreshLayout();
                return true;
            }
            return false;
        }
        
        public static int GetGridInsertIndexLocal(
            GridLayoutGroup grid,
            RectTransform container,
            Vector2 pointerLocal,
            int itemCount
        )
        {
            Rect rect = container.rect;

            // Convert to top-left origin
            float x = pointerLocal.x + rect.width * 0.5f;
            float y = -pointerLocal.y;

            // Subtract padding
            x -= grid.padding.left;
            y -= grid.padding.top;

            // Clamp inside content
            x = Mathf.Max(0, x);
            y = Mathf.Max(0, y);

            float stepX = grid.cellSize.x + grid.spacing.x;
            float stepY = grid.cellSize.y + grid.spacing.y;

            int col, row, index;

            switch (grid.constraint)
            {
                case GridLayoutGroup.Constraint.FixedColumnCount:
                {
                    int cols = grid.constraintCount;

                    col = Mathf.FloorToInt(x / stepX);
                    row = Mathf.FloorToInt(y / stepY);

                    col = Mathf.Clamp(col, 0, cols - 1);

                    index = row * cols + col;
                    break;
                }

                case GridLayoutGroup.Constraint.FixedRowCount:
                {
                    int rows = grid.constraintCount;

                    col = Mathf.FloorToInt(x / stepX);
                    row = Mathf.FloorToInt(y / stepY);

                    row = Mathf.Clamp(row, 0, rows - 1);

                    index = col * rows + row;
                    break;
                }

                default: // Flexible
                {
                    int cols = Mathf.Max(
                        1,
                        Mathf.FloorToInt(
                            (rect.width - grid.padding.horizontal + grid.spacing.x) / stepX
                        )
                    );

                    col = Mathf.FloorToInt(x / stepX);
                    row = Mathf.FloorToInt(y / stepY);

                    col = Mathf.Clamp(col, 0, cols - 1);

                    index = row * cols + col;
                    Debug.Log($"{col} {row} {index} {pointerLocal.x}/{x}/{stepX} {pointerLocal.y}/{y}/{stepY}");
                    break;
                }
            }

            // Clamp to insertion range
            return Mathf.Clamp(index, 0, itemCount);
        }


        public override void AddCard(Card card, int position=-1)
        {
            if (position == -1) position = cards.Count - 1;
            card.transform.SetParent(transform);
            var cc = Instantiate(cardPrefab, content);
            cc.card = card;
            var hcc = cc.gameObject.AddComponent<CardDisplayInteractable>();
            hcc.Init(this);
            hcc.transform.SetSiblingIndex(position);
            cards.Insert(position, hcc);
        }
    }
}