using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using World.Objects;

namespace Game.Shop
{
    //save the state!!!! as a int array !!!
    public class Shop : MonoBehaviour
    {
        public ShopInventory shopInventoryPrefab;
        public List<ShopInventory.ShopItem> shopItems;
        public string shopName;
        public string shopId;

        private void Awake()
        {
            GlobalWorldManager.RunOnNextLoad(c =>
            {
                if (shopId == "") return;
                if (GlobalState.instance.TryGetTup(shopId, out var tup))
                {
                    for (int i = 0; i < shopItems.Count; i++)
                    {
                        if (tup[i] > 0)
                        {
                            var s = shopItems[i];
                            s.sold = true;
                            shopItems[i] = s;
                        }
                    }
                }
            });
        }
        
        

        public void Interact(BaseInteractable I, GameObject player)
        {
            var canv = GameObject.FindGameObjectWithTag("Main UI");
            Instantiate(shopInventoryPrefab, canv.transform).Populate(shopName, shopItems, i =>
            {
                var item = shopItems[i];
                item.sold = true;
                shopItems[i] = item;
                Save();
            });
        }

        public void Save()
        {
            if (shopId == "") return;
            List<int> state = new List<int>();
            foreach (var item in shopItems)
            {
                state.Add(item.sold ? 1 : 0);
            }
            GlobalState.instance.SetTup(shopId, state);
        }

        // private void Update()
        // {
        //     if (Input.GetMouseButtonDown(0) && IsPointerOverUI(out var obj))
        //     {
        //         Debug.Log("Clicked UI: " + obj.name);
        //     }
        // }
        
        bool IsPointerOverUI(out GameObject uiObject)
        {
            uiObject = null;

            PointerEventData data = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);

            if (results.Count > 0)
            {
                uiObject = results[0].gameObject;
                return true;
            }

            return false;
        }
    }
}