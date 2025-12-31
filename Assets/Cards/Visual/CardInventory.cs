using Characters;
using UnityEngine;

namespace Cards.Visual
{
    public class CardInventory : MonoBehaviour
    {
        // just needs to support moving cards between layout and disp
        public HandLayout handLayout;
        public CardInteractionContainer deckLayout, tarotLayout;
        public GameObject deckMenu;
        public Agent targetAgent;
        public GameObject cardTab, tarotTab;
        private void Awake()
        {
            handLayout.CardUsed += card =>
            {
                targetAgent.SubmitCard(card);
            };
            targetAgent.OnAddCard += card =>
            {
                deckLayout.AddCard(card);
                tarotLayout.AddCard(card);
            };
            handLayout.AssignCardList(targetAgent.hand);
            deckLayout.AssignCardList(targetAgent.deck);
            tarotLayout.AssignCardList(targetAgent.deck);
        }
        
        public void SwitchCardsTab()
        {
            cardTab.SetActive(true);
            tarotTab.SetActive(false);
        }
        
        public void SwitchTarotTab()
        {
            cardTab.SetActive(false);
            tarotTab.SetActive(true);
        }

        protected virtual void Update()
        {
            if (Input.GetMouseButtonDown(0) && targetAgent.CardRequested)
            {
                handLayout.UseCard();
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleMenu();
            }
            
            for (int i = 1; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    handLayout.SetSelectedCard(i-1);
                }
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
                PlayerCamera.Instance.CursorUnlock -= "Inventory";
            }
            else
            {
                deckMenu.SetActive(true);
                handLayout.layout = HandLayout.LayoutMode.Inventory;
                deckLayout.RefreshLayout();
                handLayout.RefreshLayout();
                Cursor.lockState = CursorLockMode.None;
                PlayerCamera.Instance.CursorUnlock += "Inventory";
            }

            if (targetAgent is PlayerAgent player)
            {
                player.Stats.RecomputeStats();
            }
        }
    }
}