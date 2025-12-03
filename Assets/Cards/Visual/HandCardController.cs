using System;
using Cards.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Visual
{
    public class HandCardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public int index; // assigned by the layout
        private HandLayout _layout;
        private CardDisplay _attachedCardDisplay;
        public Card AttachedCard => _attachedCardDisplay.card;
        private void Awake()
        {
            _attachedCardDisplay = GetComponent<CardDisplay>();
        }

        public void Init(HandLayout layout, int index)
        {
            this._layout = layout;
            this.index = index;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _layout.OnCardHover(index);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _layout.OnCardExit(index);
        }
    }

}