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
            if (itemToReplace.location != InventoryLocationEnum.None)
            {
                if(itemEnemyController != null && itemEnemyController.State.Exists(s => s == StateEnum.Dead)
                    || itemEnemyController == null)
                {
                    inventoryManager.SwitchItems(item.location, item.index, item.Owner, itemToReplace.location, itemToReplace.index, itemToReplace.Owner);
                }
            }
        }
    }
}
