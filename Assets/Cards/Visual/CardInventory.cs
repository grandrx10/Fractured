using Cards.Core;
using Characters;
using UnityEngine;
using Utils;

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
        public CardPreview preview;
        private void Awake()
        {
            handLayout.CardUsed += card =>
            {
                targetAgent.SubmitCard(card);
            };
            targetAgent.OnAddCard += card =>
            {
                deckLayout.AddCard(card, false);
                tarotLayout.AddCard(card, false);
                handLayout.AddCard(card, false);
                CreatePreview(card);
            };
            targetAgent.OnRemoveCard += card =>
            {
                deckLayout.RemoveCard(card, false);
                handLayout.RemoveCard(card, false);
                tarotLayout.RemoveCard(card, false);
            };
            handLayout.AssignCardList(targetAgent.hand);
            deckLayout.AssignCardList(targetAgent.deck);
            tarotLayout.AssignCardList(targetAgent.deck);
        }
        
        public void CreatePreview(Card c)
        {
            var cardPrev = Instantiate(preview, UIHelper.GetRootCanvas(transform).transform);
            cardPrev.cardDisplay.card = c;
            cardPrev.cardDisplay.interactable = true;
            cardPrev.cardDisplay.hasDepth = true;
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
            if (!PlayerInteractController.PlayerInputs.IsInputAllowed(InputBlockPrio.Inventory)) return;
            if (Input.GetKeyDown(KeyCode.Q) && !PlayerInteractController.PlayerInputs.InCombat)
            {
                ToggleMenu();
            }
            
            if (!PlayerInteractController.PlayerInputs.IsInputAllowed(InputBlockPrio.StandardInput)) return;
            if (Input.GetMouseButtonDown(0) && targetAgent.CardRequested)
            {
                handLayout.UseCard();
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
                PlayerInteractController.PlayerInputs.RemoveBlocker("Inventory");
                PlayerCamera.Instance.CursorUnlock -= "Inventory";
            }
            else
            {
                deckMenu.SetActive(true);
                handLayout.layout = HandLayout.LayoutMode.Inventory;
                deckLayout.RefreshLayout();
                handLayout.RefreshLayout();
                PlayerInteractController.PlayerInputs.AddBlocker("Inventory", InputBlockPrio.Inventory);
                PlayerCamera.Instance.CursorUnlock += "Inventory";
            }

            if (targetAgent is PlayerAgent player)
            {
                player.UpdateStats();
            }
        }
    }
}