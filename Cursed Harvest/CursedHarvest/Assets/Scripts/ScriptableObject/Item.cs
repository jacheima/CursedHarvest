using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        plant, seed, tool
    }

    public ItemType type;

    public string itemName = "New Item";
    public Sprite icon = null;
    public bool isDefaultItem = false;

    public GameObject prefab;

   


    public virtual void Use()
    {
        Debug.Log("Using " + name);
    }
}
