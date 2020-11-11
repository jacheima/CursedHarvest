using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;

    Item item;      //current item in the slot

    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;

        if(removeButton != null)
        {
            removeButton.interactable = true;
        }
    }

    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;

        if(removeButton != null)
        {
            removeButton.interactable = false;
        }
        
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
    }

    public void EquipItem(InventorySlot slot)
    {
        if(slot != null)
        {
            Inventory.instance.EquipItem(slot.item);
        }
    }

    public void UseItem()
    {
        if(item != null)
        {
            item.Use();
        }
    }
}
