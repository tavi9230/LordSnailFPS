using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Pickable : MonoBehaviour
{
    #region Variables
    public string ItemName;

    private GameManager gameManager;
    private InventoryItem inventoryItem;
    private PlayerController playerController;
    private UIManager uiManager;
    #endregion

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        if (!string.IsNullOrEmpty(ItemName)
            && gameManager.InventoryItems.ContainsKey(ItemName)
            && inventoryItem == null)
        {
            inventoryItem = new InventoryItem(gameManager.InventoryItems[ItemName], true);
        }
    }

    private void OnMouseDown()
    {
        playerController.State.Add(StateEnum.Pickup);
        var playerMoveCollider = playerController.gameObject.transform.GetChild(0).transform.position;
        if (Vector3.Distance(transform.position, playerMoveCollider) <= 1.5f)
        {

            if (playerController.PickUp(inventoryItem))
            {
                TooltipHandler.HideTooltip(uiManager);
                Destroy(gameObject);
            }
        }
        playerController.State.Remove(StateEnum.Pickup);
    }

    public void SetInventoryItem(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;
    }

    private void OnMouseEnter()
    {
        TooltipHandler.DisplayTooltip(uiManager, inventoryItem.Name);
    }

    private void OnMouseExit()
    {
        TooltipHandler.HideTooltip(uiManager);
    }
}
