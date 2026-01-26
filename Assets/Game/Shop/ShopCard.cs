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
        private int _price;
        public void Populate(Card card, int price, bool sold, Action action)
        {
            cardDisplay.card = card;
            priceDisplay.text = price.ToString();
            _price = price;
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
            if (GlobalState.instance.Money < _price) return;
            GlobalState.instance.AddMoney(-_price);
            _onBuy?.Invoke();
            bought = true;
            soldGraphic.SetActive(true);
            OpenWorldEnv.Current.player.GiveCard(cardDisplay.card);
        }
    }
}