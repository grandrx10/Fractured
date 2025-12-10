using UnityEngine;

namespace Cards.Visual
{
    public class CardInventory : MonoBehaviour
    {
        // just needs to support moving cards between layout and disp
        public HandLayout handLayout;
        public CardInteractionContainer deckLayout;
        public GameObject deckMenu;
        public Agent targetAgent;

        private void Awake()
        {
            handLayout.CardUsed += card =>
            {
                targetAgent.SubmitCard(card);
            };
            targetAgent.OnAddCard += card =>
            {
                deckLayout.AddCard(card);
            };
            handLayout.PopulateCards(targetAgent.hand);
            deckLayout.PopulateCards(targetAgent.deck);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && targetAgent.CardRequested)
            {
                handLayout.UseCard();
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleMenu();
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                handLayout.OnScroll(Input.mouseScrollDelta.y);
            }
        }

        private void ToggleMenu()
        {
            bool deckActive = deckMenu.activeSelf;
            if (deckActive)
            {
                deckMenu.SetActive(false);
                handLayout.layout = HandLayout.LayoutMode.Hand;
                handLayout.RefreshLayout();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                deckMenu.SetActive(true);
                handLayout.layout = HandLayout.LayoutMode.Inventory;
                deckLayout.RefreshLayout();
                handLayout.RefreshLayout();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (targetAgent is PlayerAgent player)
            {
                player.Stats.RecomputeStats();
            }
        }
    }
}