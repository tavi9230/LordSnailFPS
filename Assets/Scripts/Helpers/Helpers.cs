using UnityEngine;

public static class Helpers
{
    public static Vector3 DistanceFromMouseCustom(Transform transform)
    {
        var mousePosition = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
        var distanceToPlayer = -(transform.GetChild(0).transform.position - mousePosition);

        if (distanceToPlayer.x > 0.1f)
        {
            distanceToPlayer.x = 0.11f;
        }
        else if (distanceToPlayer.x < -0.1f)
        {
            distanceToPlayer.x = -0.11f;
        }
        else if (distanceToPlayer.x < 0.1f && distanceToPlayer.x > -0.1f)
        {
            distanceToPlayer.x = 0f;
        }
        if (distanceToPlayer.y > 0.1f)
        {
            distanceToPlayer.y = 0.11f;
        }
        else if (distanceToPlayer.y < -0.1f)
        {
            distanceToPlayer.y = -0.11f;
        }
        else if (distanceToPlayer.y < 0.1f && distanceToPlayer.y > -0.1f)
        {
            distanceToPlayer.y = 0f;
        }
        return distanceToPlayer;
    }

    public static Vector3 DistanceFromMouse(Transform transform)
    {
        var mousePosition = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
        var distanceToPlayer = -(transform.GetChild(0).transform.position - mousePosition);
        return distanceToPlayer;
    }

    public static float GetPercentOfValue(float value, float percent)
    {
        return RoundToTwoDecimals(value * (percent / 100));
    }

    public static float RoundToTwoDecimals(float value)
    {
        return (float)System.Math.Round(value * 100f) / 100f;
    }

    public static string GetItemLocationString(InventoryLocationEnum itemLocation, int itemIndex)
    {
        string location;

        switch (itemLocation)
        {
            case InventoryLocationEnum.Head:
                location = "head";
                break;
            case InventoryLocationEnum.Neck:
                location = "neck";
                break;
            case InventoryLocationEnum.Torso:
                location = "torso";
                break;
            case InventoryLocationEnum.RightHand:
                location = "rightHand";
                break;
            case InventoryLocationEnum.LeftHand:
                location = "leftHand";
                break;
            case InventoryLocationEnum.Hip:
                location = "hip";
                break;
            case InventoryLocationEnum.RightRing:
                location = "rightRing";
                break;
            case InventoryLocationEnum.LeftRing:
                location = "leftRing";
                break;
            case InventoryLocationEnum.Feet:
                location = "feet";
                break;
            case InventoryLocationEnum.Inventory:
                location = "inventory" + itemIndex;
                break;
            case InventoryLocationEnum.AlternateRightHand:
                location = "alternateRightHand";
                break;
            case InventoryLocationEnum.AlternateLeftHand:
                location = "alternateLeftHand";
                break;
            case InventoryLocationEnum.Hands:
                location = "hands";
                break;
            case InventoryLocationEnum.None:
            default:
                location = null;
                break;
        }

        return location;
    }

    public static void ConsumePotion(PlayerController playerController, UIManager uiManager, KeyCode keyCode)
    {
        playerController.Hotbar[keyCode].Item.Quantity.Value--;
        var regain = playerController.Hotbar[keyCode].Item.Regain;
        var totalToRegain = Random.Range(regain.MinValue, regain.MaxValue);

        if (playerController.Hotbar[keyCode].Item.Id.Contains("consumable1") || playerController.Hotbar[keyCode].Item.Name.Contains("Healing"))
        {
            playerController.Hotbar[keyCode].Item.RegainHealth(totalToRegain + playerController.Stats.Attributes.Constitution, playerController.Stats);
        }
        if (playerController.Hotbar[keyCode].Item.Id.Contains("consumable2") || playerController.Hotbar[keyCode].Item.Name.Contains("Mana"))
        {
            playerController.Hotbar[keyCode].Item.RegainMana(totalToRegain, playerController.Stats);
        }
        if (playerController.Hotbar[keyCode].Item.Id.Contains("consumable3") || playerController.Hotbar[keyCode].Item.Name.Contains("Stamina"))
        {
            playerController.Hotbar[keyCode].Item.RegainStamina(totalToRegain, playerController.Stats);
        }

        if (playerController.Hotbar[keyCode].Item.Quantity.Value <= 0)
        {
            string location = null;
            foreach (var loc in playerController.InventoryManager.Inventory.Keys)
            {
                if (playerController.InventoryManager.Inventory[loc].Compare(playerController.Hotbar[keyCode].Item))
                {
                    location = loc;
                    break;
                }
            }

            foreach (var kc in playerController.Hotbar.Keys)
            {
                if (playerController.Hotbar[kc].Item != null && playerController.Hotbar[kc].Item.Compare(playerController.InventoryManager.Inventory[location]))
                {
                    playerController.Hotbar[kc].Item = null;
                }
            }

            playerController.InventoryManager.Inventory[location] = new InventoryItem();
        }
        uiManager.ResetHotbar();
        uiManager.RefreshInventory(null, playerController.Hotbar[keyCode].Item);
    }

    public static void ConsumePotion(PlayerController playerController, UIManager uiManager, string location)
    {
        var item = playerController.InventoryManager.Inventory[location];
        item.Quantity.Value--;
        var regain = item.Regain;
        var totalToRegain = Random.Range(regain.MinValue, regain.MaxValue);

        if (item.Id.Contains("consumable1") || item.Name.Contains("Healing"))
        {
            item.RegainHealth(totalToRegain + playerController.Stats.Attributes.Constitution, playerController.Stats);
        }
        if (item.Id.Contains("consumable2") || item.Name.Contains("Mana"))
        {
            item.RegainMana(totalToRegain, playerController.Stats);
        }
        if (item.Id.Contains("consumable3") || item.Name.Contains("Stamina"))
        {
            item.RegainStamina(totalToRegain, playerController.Stats);
        }

        if (item.Quantity.Value <= 0)
        {
            foreach (var kc in playerController.Hotbar.Keys)
            {
                if (playerController.Hotbar[kc].Item != null && playerController.Hotbar[kc].Item.Compare(item))
                {
                    playerController.Hotbar[kc].Item = null;
                }
            }

            playerController.InventoryManager.Inventory[location] = new InventoryItem();
            item = null;
        }
        uiManager.ResetHotbar();
        uiManager.RefreshInventory(null, item);
    }
}