using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            ItemStatus item = eventData.pointerDrag.GetComponent<ItemStatus>();
            ItemStatus itemToReplace = gameObject.transform.GetChild(0).GetComponent<ItemStatus>();
            EnemyController itemEnemyController = item.Owner.GetComponent<EnemyController>();
            EnemyController itemToReplaceEnemyController = itemToReplace.Owner.GetComponent<EnemyController>();

            var inventoryManager = FindObjectOfType<PlayerController>().InventoryManager;
            if (itemToReplace.location != InventoryLocationEnum.None && itemEnemyController == null && itemEnemyController == null)
            {
                // TODO: send owner down to method in order for player to be able to switch items with enemy (also do some logic there)
                inventoryManager.SwitchItems(item.location, item.index, itemToReplace.location, itemToReplace.index);
            }
        }
    }
}
