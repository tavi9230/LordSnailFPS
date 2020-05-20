using UnityEngine;
using UnityEngine.EventSystems;

public class ItemStatus : MonoBehaviour
{
    #region Variables
    public InventoryLocationEnum location { get; set; }
    public int index { get; set; }
    public InventoryItem InventoryItem { get; set; }
    public GameObject Owner { get; set; }
    public GameObject Enemy { get; set; }
    #endregion

    public void SetInvetoryItem(InventoryLocationEnum location, int index, InventoryItem inventoryItem, GameObject owner)
    {
        this.location = location;
        this.index = index;
        InventoryItem = inventoryItem;
        Owner = owner;
    }

    public void SetEnemyItem(GameObject enemy)
    {
        Enemy = enemy;
    }
}
