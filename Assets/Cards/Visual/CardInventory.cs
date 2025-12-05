using UnityEngine;

namespace Cards.Visual
{
    public class CardInventory : MonoBehaviour
    {
        // just needs to support moving cards between layout and disp
        public CardInteractionContainer handLayout;
        public CardInteractionContainer deckLayout;
        public Agent targetAgent;
        
    }
}