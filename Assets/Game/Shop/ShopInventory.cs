using System;
using System.Collections.Generic;
using Cards.Core;
using Characters;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Shop
{
    public class ShopInventory : MonoBehaviour
    {
        public List<ShopCard> items;
        public ShopCard itemPrefab;
        public RectTransform content;
        public TextMeshProUGUI shopNameText;
        public void Populate(string shopName, List<ShopItem> shopItems, Action<int> onBuy)
        {
            shopNameText.text = shopName;
            while (content.childCount > 0)
            {
                Destroy(content.GetChild(0).gameObject);
            }
            for (int i = 0; i < shopItems.Count; i++)
            {
                var item = shopItems[i];
                var c = Instantiate(itemPrefab, content);
                var index = i;
                c.Populate(item.card, item.cost, item.sold, () =>
                {
                    onBuy?.Invoke(index);
                });
                items.Add(c);
            }
            PlayerInteractController.PlayerInputs.AddBlocker($"Shop {gameObject.GetInstanceID()}", InputBlockPrio.Inventory);
            PlayerCamera.Instance.CursorUnlock += $"Shop {gameObject.GetInstanceID()}";
        }

        private void OnDestroy()
        {
            PlayerInteractController.PlayerInputs.RemoveBlocker($"Shop {gameObject.GetInstanceID()}");
            PlayerCamera.Instance.CursorUnlock -= $"Shop {gameObject.GetInstanceID()}";
        }

        private void Update()
        {
            if (!PlayerInteractController.PlayerInputs.IsInputAllowed(InputBlockPrio.Inventory)) return;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Delete();
            }
        }

        public void Delete()
        {
            Destroy(gameObject);
        }
        
        public void Clear()
        {
            foreach (ShopCard item in items)
            {
                Destroy(item.gameObject);
            }
            items.Clear();
        }
        
        [Serializable]
        public struct ShopItem
        {
            public Card card;
            public int cost;
            public bool sold;
        }
    }
}