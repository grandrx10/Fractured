using System;
using Cards.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Visual
{
    public class HandCardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public int index; // assigned by the layout
        private HandLayout layout;
        private BaseCardDisplay attachedCardDisplay;
        public Card AttachedCard => attachedCardDisplay.card;
        private void Awake()
        {
            attachedCardDisplay = GetComponent<BaseCardDisplay>();
        }

        public void Init(HandLayout layout, int index)
        {
            this.layout = layout;
            this.index = index;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            layout.OnCardHover(index);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            layout.OnCardExit(index);
        }
    }

}