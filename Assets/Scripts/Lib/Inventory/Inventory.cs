using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Inventory
{

    List<Item> m_items = new List<Item>();
    List<InventoryObserver> m_observer = new List<InventoryObserver>();

    public Inventory() { }


    public Inventory(List<Item> a_items)
    {
        m_items = a_items;
    }


    public void AddItem(Item a_item)
    {
        m_items.Add(a_item);
        Debug.Log("Adding item [Inventory] : " + a_item.Name);

        NotifyAddItem(a_item);
    }

    public bool UseItem(Item a_item)
    {
        if (!HasItem(a_item))
        {
            return false;
        }

        if (a_item.DestroyAfterUse)
        {
            RemoveItem(a_item);
        }

        Debug.Log("Use item [Inventory] : " + a_item.Name);

        return true;
    }

    public bool RemoveItem(Item a_item)
    {

        bool res = m_items.Remove(a_item);

        NotifyRemoveItem(a_item);

        return res;
    }

    public bool RemoveItem(string a_itemName)
    {
         Item item = m_items.Find((o) => o.Name == a_itemName);

        if(item == null)
        {
            return false;
        }
        

        return RemoveItem(item);

    }


    public bool HasItem(Item a_item)
    {
        return m_items.Contains(a_item);
    }


    public bool HasItem(string a_itemName)
    {
        return m_items.Find((o) => o.Name == a_itemName) != null;
    }

    public Item GetItem(string a_itemName)
    {
        return m_items.Find((o) => o.Name == a_itemName);
    }


    public void Register(InventoryObserver a_observer)
    {
        m_observer.Add(a_observer);
    }

    private void NotifyAddItem(Item a_item)
    {
        foreach(InventoryObserver observer in m_observer)
        {
            observer.ItemAdded(a_item);
        }
    }

    private void NotifyRemoveItem(Item a_item)
    {
        foreach (InventoryObserver observer in m_observer)
        {
            observer.ItemRemoved(a_item);
        }
    }



}
