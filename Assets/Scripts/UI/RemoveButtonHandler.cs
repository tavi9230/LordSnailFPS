using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveButtonHandler : MonoBehaviour, IPointerClickHandler
{
    private UIManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var mainParentName = gameObject.transform.parent.parent.parent.parent.name;
        if (mainParentName.StartsWith("PlayerInfo") && gameObject.transform.parent.name.StartsWith("Inventory"))
        {
            ItemStatus itemStatus = transform.parent.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<ItemStatus>();
            var owner = itemStatus.Owner.GetComponent<PlayerController>();

            if (itemStatus.location == InventoryLocationEnum.Inventory)
            {
                bool shouldRefreshHotbar = false;
                foreach (var key in owner.Hotbar.Keys)
                {
                    if (owner.Hotbar[key].Item != null)
                    {
                        if (owner.Hotbar[key].Item.Compare(owner.InventoryManager.Inventory[Helpers.GetItemLocationString(itemStatus.location, itemStatus.index)]))
                        {
                            owner.Hotbar[key].Item = null;
                            shouldRefreshHotbar = true;
                        }
                    }
                }
                if (shouldRefreshHotbar)
                {
                    uiManager.ResetHotbar();
                }
            }

            itemStatus.Owner.GetComponent<PlayerController>().InventoryManager.DropItem(itemStatus.location, itemStatus.index, itemStatus.Owner);
        }
        else if (mainParentName.StartsWith("EnemyInfo") && gameObject.transform.parent.name.StartsWith("Inventory"))
        {
            ItemStatus itemStatus = transform.parent.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<ItemStatus>();
            var playerPos = FindObjectOfType<PlayerController>().transform.position;
            var targetPos = itemStatus.Owner.GetComponent<EnemyController>().transform.position;
            if (Vector3.Distance(playerPos, targetPos) <= 2.5)
            {
                itemStatus.Owner.GetComponent<EnemyController>().InventoryManager.DropItem(itemStatus.location, itemStatus.index, itemStatus.Owner);
            }
        }
        else if (gameObject.transform.parent.name.StartsWith("ObjectInventory"))
        {
            GameObject pickups = GameObject.Find("Pickups");
            ItemStatus itemStatus = transform.parent.transform.GetChild(0).GetComponent<ItemStatus>();
            itemStatus.Owner.GetComponent<ObjectController>().Content.Remove(itemStatus.InventoryItem);

            var pickup = Instantiate(itemStatus.InventoryItem.GameObject, pickups.transform);
            pickup.transform.position = itemStatus.Owner.transform.position;

            uiManager.RefreshInventory(itemStatus.Owner);
        }
        else if (gameObject.transform.parent.name.StartsWith("ObjectEnemyInventory"))
        {
            ItemStatus itemStatus = transform.parent.transform.GetChild(0).GetComponent<ItemStatus>();
            if (itemStatus.Enemy != null)
            {
                itemStatus.Owner.GetComponent<ObjectController>().EnemyContent.Remove(itemStatus.Enemy);
                itemStatus.Enemy.transform.position = itemStatus.Owner.transform.position;
                itemStatus.Enemy.SetActive(true);
                itemStatus.Enemy.GetComponent<EnemyController>().Hidespot = null;
            }
            uiManager.RefreshInventory(itemStatus.Owner);
        }
        else if (gameObject.transform.parent.name.StartsWith("ActiveSkillSlot"))
        {
            PlayerController pc = FindObjectOfType<PlayerController>();
            SkillStatus ss = gameObject.transform.parent.GetChild(0).GetComponent<SkillStatus>();

            var idx = pc.Stats.Skills[ActivityEnum.Active].FindIndex(s => s.Id == ss.skill.Id);
            pc.Stats.Skills[ActivityEnum.Active].RemoveAt(idx);
            foreach (var key in pc.Hotbar.Keys)
            {
                if (pc.Hotbar[key].Skill != null && pc.Hotbar[key].Skill.Id == ss.skill.Id)
                {
                    pc.Hotbar[key].Skill = null;
                }
            }
            uiManager.RefreshInventory();
            uiManager.ResetHotbar();
        }
    }
}