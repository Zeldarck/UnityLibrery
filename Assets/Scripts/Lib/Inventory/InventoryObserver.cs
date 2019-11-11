using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InventoryObserver
{

    void ItemAdded(Item a_item);
    void ItemRemoved(Item a_item);
}
