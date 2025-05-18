using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryItem[] items;
    public Dictionary<ItemData, InventoryItem> itemDictionary = new Dictionary<ItemData, InventoryItem>();
    public InventoryPanel inventoryPanel;

    public static event Action<ItemData> OnItemAdded;

    void Start()
    {
        items = new InventoryItem[5];
    }

    public void AddItem(ItemData addedItem){
        for(int i = 0; i < items.Length; i++){
            Debug.Log("checking inventory");
            if(items[i].itemData == null){
                items[i] = new InventoryItem(addedItem);
                inventoryPanel.itemIconSlots[i].sprite = addedItem.itemIcon;
                inventoryPanel.quantityText[i].text = items[i].quantity.ToString();
                Debug.Log("adding new item");
                return;
            }else if(items[i].itemData == addedItem){
                items[i].AddToStack();
                inventoryPanel.quantityText[i].text = items[i].quantity.ToString();
                Debug.Log("raising quantity on existing item");
                return;
            }
        }
        Debug.Log("full inventory sorry");
    }

    public void RemoveItem(ItemData removedItem){
        for(int i = 0; i < items.Length; i++){
            if(items[i].itemData == removedItem){
                items[i].RemoveFromStack();
                if(items[i].quantity <= 0){
                    items[i] = null;
                }
            }
        }
    }

}