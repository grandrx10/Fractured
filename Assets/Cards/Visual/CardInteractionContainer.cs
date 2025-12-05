using System.Collections.Generic;
using Cards.Core;
using UnityEngine;

namespace Cards.Visual
{
    public class CardInteractionContainer : MonoBehaviour
    {
        public List<CardDisplayInteractable> cards;

        public virtual void PopulateCards(List<Card> c)
        {
            
        }
        
        public virtual void OnCardDropped(CardDisplayInteractable cardDisplay)
        {
            
        }
        
        public virtual void OnCardRemoved(CardDisplayInteractable cardDisplay)
        {

        }
        
        public virtual void OnCardExit(int index)
        {

        }
        public virtual void OnCardHover(int index)
        {

        }
    }
}