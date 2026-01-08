using System;
using Characters;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Visual
{
    public class CardPreview : MonoBehaviour, IPointerClickHandler
    {
        public CardDisplay cardDisplay;

        private void Awake()
        {
            PlayerCamera.Instance.CursorUnlock += $"preview {gameObject.GetInstanceID()}";
            PlayerInteractController.PlayerInputs.AddBlocker($"preview {gameObject.GetInstanceID()}", InputBlockPrio.Menu);
        }

        private void OnDestroy()
        {
            PlayerCamera.Instance.CursorUnlock -= $"preview {gameObject.GetInstanceID()}";
            PlayerInteractController.PlayerInputs.RemoveBlocker($"preview {gameObject.GetInstanceID()}");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Destroy(gameObject);
        }
    }
}