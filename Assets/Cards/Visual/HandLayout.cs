using System;
using UnityEngine;
using System.Collections.Generic;
using Cards.Core;
using UnityEngine.Serialization;

namespace Cards.Visual
{
    public class HandLayout : MonoBehaviour
    {
        public InteractMode mode;
        public LayoutMode layout;
        public RectTransform content;
        [SerializeField] private List<HandCardController> cards = new List<HandCardController>();
        [SerializeField] private float cardWidth = 150f;
        [SerializeField] private float maxSpacing = 150f;
        [SerializeField] private float baseY = 80f;
        [SerializeField] private float hoverPopHeight = 140f;
        [SerializeField] private float sideShift = 40f;
        [SerializeField] private float sideMove = 40f;
        [SerializeField] private float animTime = 0.15f;
        public Action<Card> CardUsed;
        public CardDisplay cardPrefab;
        
        public enum LayoutMode
        {
            Hand,
            Inventory
        }
        
        private int _hoveredIndex = -1;
        private RectTransform _rect;

        public enum InteractMode
        {
            CardGame,
            ThirdPerson,
            Inactive
        }

        public void PopulateCards(List<Card> c)
        {
            _rect = GetComponent<RectTransform>();
            for (int i = 0; i < c.Count; i++)
            {
                var cc = Instantiate(cardPrefab, content);
                cc.card = c[i];
                var hcc = cc.gameObject.AddComponent<HandCardController>();
                hcc.Init(this, i);
                cards.Add(hcc);
            }
            RefreshLayout();
        }

        public void UseCard()
        {
            if (_hoveredIndex == -1) return;
            CardUsed.Invoke(cards[_hoveredIndex].AttachedCard);
        }

        public void OnCardHover(int index)
        {
            if (mode != InteractMode.CardGame || layout == LayoutMode.Inventory) return;
            _hoveredIndex = index;
            RefreshLayout();
        }

        public void OnScroll(float d)
        {
            if (mode != InteractMode.ThirdPerson || d == 0 || layout == LayoutMode.Inventory) return;
            _hoveredIndex -= (int)Mathf.Sign(d);
            if (_hoveredIndex < 0) _hoveredIndex = 0;
            if (_hoveredIndex >= cards.Count) _hoveredIndex = cards.Count - 1;
            RefreshLayout();
        }

        public void OnCardExit(int index)
        {
            if (mode != InteractMode.CardGame || layout == LayoutMode.Inventory) return;
            if (_hoveredIndex == index)
                _hoveredIndex = -1;
            RefreshLayout();
        }
        
        [ContextMenu("Refresh Layout")]
        public void RefreshLayout()
        {
            if (layout == LayoutMode.Hand)
                LayoutHandMode();
            else
                LayoutInventoryMode();
        }

        void LayoutHandMode()
        {
            if (cards.Count == 0)
                return;

            float handWidth = _rect.rect.width;
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, handWidth);
            // Evenly space cards across hand width
            float totalSpacing = handWidth - cardWidth;
            float spacing = (cards.Count > 1)
                ? totalSpacing / (cards.Count - 1)
                : 0f;
            spacing = Mathf.Min(maxSpacing, spacing);

            for (int i = 0; i < cards.Count; i++)
            {
                RectTransform rt = cards[i].GetComponent<RectTransform>();

                float targetX = i * spacing + cardWidth / 2;
                float targetY = baseY;

                if (_hoveredIndex != -1)
                {
                    if (i == _hoveredIndex)
                    {
                        // POP UP
                        targetX += sideMove;
                        targetY = hoverPopHeight;
                    }
                    else
                    {
                        // Distance-based shift
                        int dist = Mathf.Abs(i - _hoveredIndex);

                        // Weight = 1.0 for adjacent, fades to 0.0 for far cards
                        float weight = 1f / (dist + 0.5f);

                        float shiftAmount = sideShift * weight;

                        if (i < _hoveredIndex)
                        {
                            targetX -= shiftAmount;
                        }
                        else
                        {
                            targetX += shiftAmount;
                        }
                    }
                }

                // Smooth animation
                LeanTween.moveLocal(rt.gameObject, new Vector3(targetX, targetY, 0), animTime)
                    .setEaseOutQuad();
            }
        }
        
        void LayoutInventoryMode()
        {
            float cardCount = cards.Count;

            if (cardCount == 0)
                return;

            // total width of all cards placed side-by-side
            float totalWidth = cardCount * cardWidth;

            // apply this width to the Content rect so scrolling works
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);

            for (int i = 0; i < cards.Count; i++)
            {
                float x = i * cardWidth + cardWidth * 0.5f; // center each card
                float y = baseY;

                LeanTween.moveLocal(
                    cards[i].gameObject,
                    new Vector3(x, y, 0),
                    animTime
                ).setEaseOutQuad();
            }
        }
    }
}