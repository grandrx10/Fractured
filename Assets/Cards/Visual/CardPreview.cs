using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Visual
{
    public class CardPreview : MonoBehaviour, IPointerClickHandler
    {
        public CardDisplay cardDisplay;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Destroy(gameObject);
        }
    }
}