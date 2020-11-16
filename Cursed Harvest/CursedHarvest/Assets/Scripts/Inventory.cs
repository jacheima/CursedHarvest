using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int space = 20;

    public List<Item> items = new List<Item>();

    public List<Item> equippedItems = new List<Item>();

    float equippedSpace = 9;


    public bool Add(Item item)
    {
        if (!item.isDefaultItem)
        {
            if (items.Count >= space)
            {
                Debug.Log("Not enough room!");
                return false;
            }

            items.Add(item);

            if(onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
            
        }

        return true;
    }

    public bool EquipItem(Item item)
    {
        if(!item.isDefaultItem)
        {
            if(equippedItems.Count < equippedSpace)
            {
                equippedItems.Add(item);
                items.Remove(item);
            }
            else
            {
                return false;
            }

            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
        }

        return true;

        
    }

    public void RemoveEquippedItem(Item item)
    {
        equippedItems.Remove(item);

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    public void Remove(Item item)
    {
        items.Remove(item);

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    public bool HasSeedEquiped()
    {
        for(int i = 0; i < equippedItems.Count; i++)
        {
            if(equippedItems[i].type == Item.ItemType.seed)
            {
                return true;
            }
        }

        return false;
    }

    public Item GetEquippedItemByType(Item.ItemType itemType)
    {
        for(int i = 0; i < equippedItems.Count; i++)
        {
            if(equippedItems[i].type == itemType)
            {
                return equippedItems[i];
            }
        }

        return null;
    }
}
