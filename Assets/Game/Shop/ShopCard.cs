using System;
using Cards.Core;
using Cards.Environments;
using Cards.Visual;
using TMPro;
using UnityEngine;

namespace Game.Shop
{
    public class ShopCard : MonoBehaviour
    {
        public CardDisplay cardDisplay;
        public TextMeshProUGUI priceDisplay;
        public GameObject soldGraphic;
        public bool bought;
        private Action _onBuy;
        public void Populate(Card card, int price, bool sold, Action action)
        {
            cardDisplay.card = card;
            priceDisplay.text = price.ToString();
            _onBuy = action;
            if (sold)
            {
                bought = true;
                soldGraphic.SetActive(true);
            }
            else
            {
                soldGraphic.SetActive(false);
            }
        }

        public void Buy()
        {
            if (bought) return;
            _onBuy?.Invoke();
            bought = true;
            soldGraphic.SetActive(true);
            OpenWorldEnv.Current.player.AddCard(cardDisplay.card);
        }
    }
}