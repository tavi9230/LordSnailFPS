using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButtonEnterHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool IsHoveringLeftHand = false;
    private bool IsHoveringRightHand = false;
    private bool IsHoveringInventory = false;
    private PlayerController playerController;
    private ItemStatus itemStatus;
    private UIManager uiManager;

    public void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        itemStatus = transform.GetChild(0).GetComponent<ItemStatus>();
        uiManager = FindObjectOfType<UIManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemStatus.location == InventoryLocationEnum.LeftHand)
        {
            IsHoveringLeftHand = true;
        }
        if (itemStatus.location == InventoryLocationEnum.RightHand)
        {
            IsHoveringRightHand = true;
        }
        if (itemStatus.location == InventoryLocationEnum.Inventory)
        {
            IsHoveringInventory = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsHoveringLeftHand = false;
        IsHoveringRightHand = false;
        IsHoveringInventory = false;
    }

    public void Update()
    {
        if (IsHoveringLeftHand && Input.GetKeyDown(KeyCode.F1))
        {
            playerController.Stats.LeftHandAttack.Attack.Replace(itemStatus.InventoryItem);
            uiManager.RefreshInventory();
        }
        if (IsHoveringRightHand && Input.GetKeyDown(KeyCode.F2))
        {
            playerController.Stats.RightHandAttack.Attack.Replace(itemStatus.InventoryItem);
            uiManager.RefreshInventory();
        }
        if (IsHoveringInventory)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SetHotbarItem(KeyCode.Alpha0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetHotbarItem(KeyCode.Alpha1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetHotbarItem(KeyCode.Alpha2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetHotbarItem(KeyCode.Alpha3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetHotbarItem(KeyCode.Alpha4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetHotbarItem(KeyCode.Alpha5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SetHotbarItem(KeyCode.Alpha6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                SetHotbarItem(KeyCode.Alpha7);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                SetHotbarItem(KeyCode.Alpha8);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                SetHotbarItem(KeyCode.Alpha9);
            }

            if (Input.GetMouseButtonDown(1))
            {
                var invLocation = Helpers.GetItemLocationString(itemStatus.location, itemStatus.index);

                if (playerController.InventoryManager.Inventory[invLocation].Type == ItemTypeEnum.Consumable)
                {
                    Helpers.ConsumePotion(playerController, uiManager, invLocation);
                }
            }
        }
    }

    private void SetHotbarItem(KeyCode keyCode)
    {
        var invLocation = Helpers.GetItemLocationString(itemStatus.location, itemStatus.index);

        if (playerController.InventoryManager.Inventory[invLocation].Type == ItemTypeEnum.Consumable)
        {
            playerController.Hotbar[keyCode] = new HotbarItem()
            {
                Item = playerController.InventoryManager.Inventory[invLocation]
            };

            uiManager.ResetHotbar();
        }
    }
}
