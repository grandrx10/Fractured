using System;
using UnityEngine;
using System.Collections.Generic;
using Cards.Core;
using Extras.LeanTween.Framework;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace Cards.Visual
{
    public class HandLayout : CardInteractionContainer
    {
        public InteractMode mode;
        public LayoutMode layout;
        public int cardCap = 7;
        [SerializeField] private float cardWidth = 150f;
        [SerializeField] private float handWidth = 150f;
        [SerializeField] private float baseY = 80f;
        [SerializeField] private float arcXOffset = 80f;
        [SerializeField] private float hoverPopHeight = 140f;
        [SerializeField] private float sideShift = 40f;
        [SerializeField] private float animTime = 0.15f;
        
        [SerializeField] float arcRadius = 400f;
        [SerializeField] float arcAngle = 25f; // degrees left/right from center
        [SerializeField] float cardRotation = 10f;
        public Action<Card> CardUsed;
        
        public enum LayoutMode
        {
            Hand,
            Inventory
        }
        
        private CardDisplayInteractable _selectedCard;
        public CardDisplayInteractable SelectedCard => _selectedCard;
        private RectTransform _rect;

        public enum InteractMode
        {
            CardGame,
            ThirdPerson,
            Inactive
        }

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }
        
        public void UseCard()
        {
            if (_selectedCard == null) return;
            CardUsed.Invoke(_selectedCard.AttachedCard);
        }

        public override void OnCardStartHover(CardDisplayInteractable card)
        {
            if (mode != InteractMode.CardGame || layout == LayoutMode.Inventory) return;
            _selectedCard = card;
            RefreshLayout();
        }

        public void OnScroll(float d)
        {
            if (mode != InteractMode.ThirdPerson || d == 0 || layout == LayoutMode.Inventory) return;
            var hoveredIndex = CardDisplays.IndexOf(_selectedCard);
            hoveredIndex -= (int)Mathf.Sign(d);
            if (hoveredIndex < 0) hoveredIndex += CardDisplays.Count;
            if (hoveredIndex >= CardDisplays.Count) hoveredIndex -= CardDisplays.Count;
            if (hoveredIndex > -1 && hoveredIndex < CardDisplays.Count) _selectedCard = CardDisplays[hoveredIndex];
            RefreshLayout();
        }

        public override void AddCardDisplay(Card card, int position = 0)
        {
            base.AddCardDisplay(card, position);
            RefreshLayout();
        }

        public override bool OnCardDropped(CardInteractionContainer source, CardDisplayInteractable card)
        {
            if (CardDisplays.Count >= cardCap) return false;
            return base.OnCardDropped(source, card);
        }

        private int HoveredIndexClamped()
        {
            return Mathf.Max(CardDisplays.IndexOf(_selectedCard), 0);
        }

        public override void OnCardStopHover(CardDisplayInteractable card)
        {
            if (mode != InteractMode.CardGame || layout == LayoutMode.Inventory) return;
            if (_selectedCard == card)
                _selectedCard = null;
            RefreshLayout();
        }
        
        [ContextMenu("Refresh Layout")]
        public override void RefreshLayout()
        {
            ValidateSelectedCard();
            if (layout == LayoutMode.Hand)
                LayoutHandMode();
            else
                LayoutInventoryMode();
        }
        
        private void ValidateSelectedCard()
        {
            if (_selectedCard && !CardDisplays.Contains(_selectedCard)) _selectedCard = null;
        }

        public void SetSelectedCard(int index)
        {
            if (layout == LayoutMode.Hand && index >= 0 && index < CardDisplays.Count)
            {
                _selectedCard = CardDisplays[index];
                RefreshLayout();
            }
        }
        
        int CircularOffset(int i, int center, int count)
        {
            int offset = i - center;
            int d = Mathf.FloorToInt(count / 2f);
            if (offset > d)
                offset -= count;
            else if (offset < -d)
                offset += count;

            return offset;
        }

        
        private void LayoutHandMode()
        {
            int count = CardDisplays.Count;
            if (count == 0)
                return;
            
            int centerIndex = HoveredIndexClamped();

            int maxOffset = Mathf.Max(1, count / 2);

            for (int i = 0; i < count; i++)
            {
                RectTransform rt = CardDisplays[i].GetComponent<RectTransform>();

                // Circular distance from center
                int offset = CircularOffset(i, centerIndex, count);

                // Normalize to [-1, 1] but CLAMP to arc
                float t = Mathf.Clamp((float)offset / maxOffset, -1f, 1f);

                // Arc math
                float angle = t * arcAngle * Mathf.Deg2Rad;
                float x = Mathf.Sin(angle) * arcRadius + arcXOffset;
                float y = baseY + Mathf.Cos(angle) * arcRadius - arcRadius;

                // Rotation
                float rotZ = -t * cardRotation;

                if (!_selectedCard)
                {
                    SetSelectedCard(0);
                }

                if (_selectedCard != null)
                {
                    if (i == centerIndex)
                    {
                        y += hoverPopHeight;
                    }
                    else
                    {
                        float weight = 1f / (Mathf.Abs(offset) + 0.5f);
                        x += Mathf.Sign(offset) * sideShift * weight;
                    }
                }

                // Animate position
                LeanTween.moveLocal(rt.gameObject, new Vector3(x, y, 0f), animTime)
                    .setEaseOutQuad();

                // Animate rotation
                LeanTween.rotateZ(rt.gameObject, rotZ, animTime)
                    .setEaseOutQuad();

                // ---- SIBLING ORDER ----
                // middle = 0, left = odd, right = even
                int priority;
                if (offset == 0)
                    priority = 0;
                else if (offset < 0)
                    priority = (-offset * 2) - (centerIndex < count / 2 ? 0 : 1);
                else
                    priority = offset * 2 - (centerIndex < count / 2 ? 1 : 0);

                int siblingIndex = count - 1 - priority;
                rt.SetSiblingIndex(siblingIndex);
            }
        }
        
        void LayoutInventoryMode()
        {
            float cardCount = CardDisplays.Count;

            if (cardCount == 0)
                return;

            // total width of all cards placed side-by-side
            float allocWidth = handWidth/(cardCount+1);

            for (int i = 0; i < CardDisplays.Count; i++)
            {
                RectTransform rt = CardDisplays[i].GetComponent<RectTransform>();
                float x = i * allocWidth + cardWidth * 0.5f; // center each card
                float y = baseY;
                rt.SetSiblingIndex(i);
                LeanTween.moveLocal(
                    CardDisplays[i].gameObject,
                    new Vector3(x, y, 0),
                    animTime
                ).setEaseOutQuad();
                LeanTween.rotateZ(CardDisplays[i].gameObject, 0, animTime)
                    .setEaseOutQuad();
            }
        }
    }
}