using System;
using Cards.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cards.Visual
{
    public class CardDisplayInteractable : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        private CardInteractionContainer _container;
        private CardDisplay _attachedCardDisplay;

        private CanvasGroup _canvasGroup;
        private Transform _originalParent;
        private Canvas _dragCanvas;
        public bool dragAllowed = true;
        public Card AttachedCard => _attachedCardDisplay.card;

        public void Init(CardInteractionContainer layout)
        {
            _container = layout;
            _attachedCardDisplay = GetComponent<CardDisplay>();
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // --------------------------
        // HOVER
        // --------------------------
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isDragging)
                _container.OnCardStartHover(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isDragging)
                _container.OnCardStopHover(this);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            _container.OnCardClick(this);
        }

        // --------------------------
        // DRAG
        // --------------------------
        private bool _isDragging = false;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!dragAllowed) return;
            _isDragging = true;
            _container.OnCardStopHover(this);     // stop hover effects
            _canvasGroup.blocksRaycasts = false;

            // Find a top-level canvas to drag under
            _dragCanvas = GetRootCanvas();
            _originalParent = transform.parent;

            transform.SetParent(_dragCanvas.transform, true);
            //print(transform.parent);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragAllowed) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _dragCanvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var localPos
            );

            transform.localPosition = localPos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragAllowed) return;
            _isDragging = false;
            _canvasGroup.blocksRaycasts = true;

            // Try find drop target
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            CardInteractionContainer dropTarget = null;
            foreach (var hit in results)
            {
                dropTarget = hit.gameObject.GetComponent<CardInteractionContainer>();
                if (dropTarget != null)
                    break;
            }
            
            bool handled = false;
            if (dropTarget != null)
            {
                handled = dropTarget.OnCardDropped(_container, this);
                
            }

            // Return back to original layout (HandLayout will re-position it)
            if (!handled)
            {
                transform.SetParent(_originalParent, true);
                _container.RefreshLayout();
            }

            //_container.RequestRebuild();
        }

        private Canvas GetRootCanvas()
        {
            Transform t = transform;
            while (t != null)
            {
                Canvas c = t.GetComponent<Canvas>();
                if (c != null && c.isRootCanvas)
                    return c;
                t = t.parent;
            }

            // fallback
            return FindObjectOfType<Canvas>();
        }
    }
}
