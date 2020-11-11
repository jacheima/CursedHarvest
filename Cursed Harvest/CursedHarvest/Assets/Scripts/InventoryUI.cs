using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public Transform equippedItemParent;
    public GameObject inventoryUI;

    Inventory inventory;

    InventorySlot[] slots;

    public InventorySlot[] equipedSlots;

    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        equipedSlots = equippedItemParent.GetComponentsInChildren<InventorySlot>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }

    void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if(i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }

        for(int i = 0; i < equipedSlots.Length; i++)
        {
            if(i < inventory.equippedItems.Count)
            {
                equipedSlots[i].AddItem(inventory.equippedItems[i]);
            }
            else
            {
                equipedSlots[i].ClearSlot();
            }
        }
    }
}
