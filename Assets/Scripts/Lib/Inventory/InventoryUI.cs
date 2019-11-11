using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : Singleton<InventoryUI>, InventoryObserver
{
    [SerializeField]
    PlayerController m_player;

    Inventory m_inventory;

    [SerializeField]
    GameObject m_itemPrefab;

    [SerializeField]
    Transform m_container;

    public void ItemAdded(Item a_item)
    {
        //TODO : create a good receipt class for items ui
        Instantiate(m_itemPrefab, m_container);
    }

    public void ItemRemoved(Item a_item)
    {
        //TODO : remove the good itemui
        Destroy(m_container.GetChild(0).gameObject);
    }

    private void Start()
    {
        m_inventory = m_player.Inventory;
        m_inventory.Register(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            m_container.gameObject.SetActive(!m_container.gameObject.activeSelf);
        }

    }
}
