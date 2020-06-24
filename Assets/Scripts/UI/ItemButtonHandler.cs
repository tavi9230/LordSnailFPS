using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButtonHandler : MonoBehaviour, IPointerClickHandler
{
    private ItemStatus itemStatus;

    void Start()
    {
        itemStatus = GetComponent<ItemStatus>();
        // in case it's the Enemy Info
        if (itemStatus == null)
        {
            itemStatus = transform.GetChild(0).GetComponent<ItemStatus>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        EnemyController enemyController = itemStatus.Owner.GetComponent<EnemyController>();
        UIManager uiManager = FindObjectOfType<UIManager>();
        // if object inventory and clicked on InventoryItem
        if (itemStatus.InventoryItem != null && enemyController == null)
        {
            if (playerController.PickUp(itemStatus.InventoryItem))
            {
                itemStatus.Owner.GetComponent<ObjectController>().Content.Remove(itemStatus.InventoryItem);
                uiManager.RefreshInventory(itemStatus.Owner);
            }
        }
        // if enemy inventory and dead
        if (enemyController != null && enemyController.State.Exists(s => s == StateEnum.Dead)
            && Vector3.Distance(playerController.transform.position, enemyController.transform.position) <= 2.5)
        {
            if (playerController.PickUp(itemStatus.InventoryItem))
            {
                // TODO: Check this out. Remove item works with player controller. It should either be controller neutral or pass the controller?
                enemyController.InventoryManager.RemoveItem(itemStatus.location, itemStatus.index, enemyController.gameObject);
                uiManager.RefreshInventory(itemStatus.Owner);
            }
        }
    }
}