using System;
using UnityEngine;
using System.Collections.Generic;
using Cards.Core;
using UnityEngine.Serialization;

namespace Cards.Visual
{
    public class HandLayout : MonoBehaviour
    {
        public LayoutMode mode;
        [SerializeField] private List<HandCardController> cards = new List<HandCardController>();
        [SerializeField] private float cardWidth = 150f;
        [SerializeField] private float maxSpacing = 150f;
        [SerializeField] private float baseY = 80f;
        [SerializeField] private float hoverPopHeight = 140f;
        [SerializeField] private float sideShift = 40f;
        [SerializeField] private float sideMove = 40f;
        [SerializeField] private float animTime = 0.15f;
        public Action<Card> CardUsed;
        public BaseCardDisplay cardPrefab;
        
        private int hoveredIndex = -1;
        private RectTransform rect;

        public enum LayoutMode
        {
            CardGame,
            ThirdPerson,
            Inactive
        }
        public void PopulateCards(List<Card> c)
        {
            rect = GetComponent<RectTransform>();
            for (int i = 0; i < c.Count; i++)
            {
                var cc = Instantiate(cardPrefab, transform);
                cc.card = c[i];
                var hcc = cc.gameObject.AddComponent<HandCardController>();
                hcc.Init(this, i);
                cards.Add(hcc);
            }
            RefreshLayout();
        }

        public void UseCard()
        {
            if (hoveredIndex == -1) return;
            CardUsed.Invoke(cards[hoveredIndex].AttachedCard);
        }

        public void OnCardHover(int index)
        {
            if (mode != LayoutMode.CardGame) return;
            hoveredIndex = index;
            RefreshLayout();
        }

        public void OnScroll(float d)
        {
            if (mode != LayoutMode.ThirdPerson || d == 0) return;
            hoveredIndex -= (int)Mathf.Sign(d);
            if (hoveredIndex < 0) hoveredIndex = 0;
            if (hoveredIndex >= cards.Count) hoveredIndex = cards.Count - 1;
            RefreshLayout();
        }

        public void OnCardExit(int index)
        {
            if (mode != LayoutMode.CardGame) return;
            if (hoveredIndex == index)
                hoveredIndex = -1;
            RefreshLayout();
        }

        void RefreshLayout()
        {
            if (cards.Count == 0)
                return;

            float handWidth = rect.rect.width;

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

                if (hoveredIndex != -1)
                {
                    if (i == hoveredIndex)
                    {
                        // POP UP
                        targetX += sideMove;
                        targetY = hoverPopHeight;
                    }
                    else
                    {
                        // Distance-based shift
                        int dist = Mathf.Abs(i - hoveredIndex);

                        // Weight = 1.0 for adjacent, fades to 0.0 for far cards
                        float weight = 1f / (dist + 0.5f);

                        float shiftAmount = sideShift * weight;

                        if (i < hoveredIndex)
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
    }

}