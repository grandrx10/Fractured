using System;
using Cards.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cards.Visual
{
    public class CardDisplayInteractable : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int index;
        private CardInteractionContainer _container;
        private CardDisplay _attachedCardDisplay;

        private CanvasGroup _canvasGroup;
        private Transform _originalParent;
        private Canvas _dragCanvas;

        public Card AttachedCard => _attachedCardDisplay.card;

        private void Awake()
        {
            _attachedCardDisplay = GetComponent<CardDisplay>();
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public void Init(CardInteractionContainer layout, int index)
        {
            this._container = layout;
            this.index = index;
        }

        // --------------------------
        // HOVER
        // --------------------------
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isDragging)
                _container.OnCardHover(index);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isDragging)
                _container.OnCardExit(index);
        }

        // --------------------------
        // DRAG
        // --------------------------
        private bool _isDragging = false;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            _container.OnCardExit(index);     // stop hover effects
            _canvasGroup.blocksRaycasts = false;

            // Find a top-level canvas to drag under
            _dragCanvas = GetRootCanvas();
            _originalParent = transform.parent;

            transform.SetParent(_dragCanvas.transform, true);
            print(transform.parent);
        }

        public void OnDrag(PointerEventData eventData)
        {
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

            if (dropTarget != null)
            {
                dropTarget.OnCardDropped(this);
            }

            // Return back to original layout (HandLayout will re-position it)
            transform.SetParent(_originalParent, false);

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
