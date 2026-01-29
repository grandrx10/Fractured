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
        public CardPreview getPreview, preview;
        private void Awake()
        {
            handLayout.CardUsed += card =>
            {
                targetAgent.SubmitCard(card);
            };
            targetAgent.OnGetCard += (card, target) =>
            {
                deckLayout.AddCardDisplay(card);
                tarotLayout.AddCardDisplay(card);
                handLayout.AddCardDisplay(card);
                CreatePreview(card, getPreview);
            };
            targetAgent.OnLoseCard += (card, target) =>
            {
                deckLayout.RemoveCardDisplay(card);
                handLayout.RemoveCardDisplay(card);
                tarotLayout.RemoveCardDisplay(card);
            };
            
            handLayout.AssignCardList(targetAgent.hand, c => { targetAgent.AddCard(c, true); },
                targetAgent.RemoveCard);
            deckLayout.AssignCardList(targetAgent.deck, c => { targetAgent.AddCard(c); },
                targetAgent.RemoveCard);
            tarotLayout.AssignCardList(targetAgent.deck, c => { targetAgent.AddCard(c); },
                targetAgent.RemoveCard);
        }
        
        public void CreatePreview(Card c, CardPreview p)
        {
            var cardPrev = Instantiate(p, UIHelper.GetRootCanvas(transform).transform);
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
            if (Input.GetMouseButtonDown(1) && handLayout.SelectedCard)
            {
                CreatePreview(handLayout.SelectedCard.AttachedCard, preview);
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

        public void MenuOff()
        {
            if (deckMenu.activeSelf) ToggleMenu();
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