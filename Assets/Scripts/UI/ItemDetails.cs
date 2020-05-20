using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDetails : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UIManager uiManager;
    private InventoryManager inventoryManager;
    private GameObject detailsObject;
    private ItemTypeEnum inspectedItemType;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        inventoryManager = FindObjectOfType<PlayerController>().InventoryManager;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var itemStatus = gameObject.GetComponent<ItemStatus>();
        if (itemStatus.location != InventoryLocationEnum.None
            && itemStatus.location != InventoryLocationEnum.ObjectEnemyInventory)
        {
            var size = uiManager.ItemDetailsUI.transform.GetComponent<RectTransform>().sizeDelta;
            uiManager.ItemDetailsUI.SetActive(true);
            inspectedItemType = itemStatus.InventoryItem.Type;
            SelectDetailsObjectAndShowItemDetails(itemStatus.InventoryItem);
        }
    }

    public void SelectDetailsObjectAndShowItemDetails(InventoryItem item)
    {
        uiManager.ItemDetailsUI.SetActive(true);
        switch (item.Type)
        {
            case ItemTypeEnum.Head:
            case ItemTypeEnum.Armor:
            case ItemTypeEnum.Shield:
            case ItemTypeEnum.Hands:
            case ItemTypeEnum.Hip:
            case ItemTypeEnum.Feet:
                detailsObject = uiManager.ItemDetailsUI.transform.GetChild(2).gameObject;
                break;
            case ItemTypeEnum.Ammo:
                detailsObject = uiManager.ItemDetailsUI.transform.GetChild(4).gameObject;
                break;
            case ItemTypeEnum.Consumable:
                detailsObject = uiManager.ItemDetailsUI.transform.GetChild(5).gameObject;
                break;
            case ItemTypeEnum.Neck:
            case ItemTypeEnum.Ring:
                detailsObject = uiManager.ItemDetailsUI.transform.GetChild(3).gameObject;
                break;
            case ItemTypeEnum.Weapon:
                detailsObject = uiManager.ItemDetailsUI.transform.GetChild(1).gameObject;
                break;
            default:
                detailsObject = null;
                break;
        }
        //gameManager.ItemDetailsUI.transform.position = gameObject.transform.position + new Vector3(size.x / 2 + 30, -size.y / 4, 0);
        if (detailsObject != null)
        {
            ShowDetails(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideAllDetails();
        inspectedItemType = ItemTypeEnum.None;
    }

    public void RefreshDetails(InventoryItem item)
    {
        if (item != null)
        {
            SelectDetailsObjectAndShowItemDetails(item);
        }
    }

    public void HideAllDetails()
    {
        var itemStatus = gameObject.GetComponent<ItemStatus>();
        HideSpecialStatsDetails();
        if (itemStatus.location != InventoryLocationEnum.None && detailsObject != null)
        {
            Transform d;
            switch (inspectedItemType)
            {
                case ItemTypeEnum.Ammo:
                    HideInsideSpecialStats();
                    break;
                case ItemTypeEnum.Consumable:
                    HideInsideSpecialStats();
                    break;
                case ItemTypeEnum.Hip:
                    HideInsideSpecialStats();
                    var inventorySlotsDetails = detailsObject.transform.GetChild(6);
                    inventorySlotsDetails.gameObject.SetActive(false);
                    break;
                case ItemTypeEnum.Feet:
                    HideInsideSpecialStats();
                    d = detailsObject.transform.GetChild(detailsObject.transform.childCount - 2);
                    for (var i = 0; i < d.childCount; i++)
                    {
                        d.GetChild(i).gameObject.SetActive(false);
                    }
                    var speedDetails = detailsObject.transform.GetChild(7);
                    speedDetails.gameObject.SetActive(false);
                    break;
                case ItemTypeEnum.Hands:
                    HideInsideSpecialStats();
                    d = detailsObject.transform.GetChild(detailsObject.transform.childCount - 2);
                    for (var i = 0; i < d.childCount; i++)
                    {
                        d.GetChild(i).gameObject.SetActive(false);
                    }
                    var critChanceDetails = detailsObject.transform.GetChild(8);
                    critChanceDetails.gameObject.SetActive(false);
                    break;
                case ItemTypeEnum.Armor:
                case ItemTypeEnum.Head:
                case ItemTypeEnum.Shield:
                    d = detailsObject.transform.GetChild(detailsObject.transform.childCount - 2);
                    for (var i = 0; i < d.childCount; i++)
                    {
                        d.GetChild(i).gameObject.SetActive(false);
                    }
                    HideInsideSpecialStats();
                    break;
                case ItemTypeEnum.Neck:
                case ItemTypeEnum.Ring:
                    HideInsideSpecialStats();
                    break;
                default:
                    break;
            }

            for (var i = 0; i < uiManager.ItemDetailsUI.transform.childCount; i++)
            {
                if (i > 0)
                {
                    uiManager.ItemDetailsUI.transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            uiManager.ItemDetailsUI.SetActive(false);

            detailsObject.SetActive(false);
            detailsObject = null;
        }
    }

    private void HideSpecialStatsDetails()
    {
        var specialStats = uiManager.ItemDetailsUI.transform.GetChild(0);
        var attributes = specialStats.transform.GetChild(0);
        var resistances = specialStats.transform.GetChild(1).gameObject;
        var damages = specialStats.transform.GetChild(2);

        for (var i = 0; i < attributes.childCount; i++)
        {
            attributes.GetChild(i).gameObject.SetActive(false);
        }

        resistances.SetActive(false);

        for (var i = 0; i < damages.childCount; i++)
        {
            damages.GetChild(i).gameObject.SetActive(false);
        }
        damages.gameObject.SetActive(false);
    }

    private void HideInsideSpecialStats()
    {
        var specialStats = detailsObject.transform.GetChild(detailsObject.transform.childCount - 1);
        for (var i = 0; i < specialStats.childCount; i++)
        {
            specialStats.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void ShowSpecialStats(InventoryItem item)
    {
        if (item.SpecialStats != null)
        {
            var specialStatsDetails = detailsObject.transform.GetChild(detailsObject.transform.childCount - 1);

            if (item.SpecialStats.Health != 0)
            {
                specialStatsDetails.GetChild(0).GetChild(1).GetComponent<Text>().text = string.Format("{0}", item.SpecialStats.Health);
                specialStatsDetails.GetChild(0).gameObject.SetActive(true);
            }
            if (item.SpecialStats.Stamina != 0)
            {
                specialStatsDetails.GetChild(1).GetChild(1).GetComponent<Text>().text = string.Format("{0}", item.SpecialStats.Stamina);
                specialStatsDetails.GetChild(1).gameObject.SetActive(true);
            }
            if (item.SpecialStats.Speed != 0)
            {
                specialStatsDetails.GetChild(2).GetChild(1).GetComponent<Text>().text = string.Format("{0}%", item.SpecialStats.Speed);
                specialStatsDetails.GetChild(2).gameObject.SetActive(true);
            }
            if (item.SpecialStats.AttackSpeedPercent != 0)
            {
                specialStatsDetails.GetChild(3).GetChild(1).GetComponent<Text>().text = string.Format("{0}%", item.SpecialStats.AttackSpeedPercent);
                specialStatsDetails.GetChild(3).gameObject.SetActive(true);
            }
            if (item.SpecialStats.StaminaConsumptionPercent != 0)
            {
                specialStatsDetails.GetChild(4).GetChild(1).GetComponent<Text>().text = string.Format("{0}%", item.SpecialStats.StaminaConsumptionPercent);
                specialStatsDetails.GetChild(4).gameObject.SetActive(true);
            }

            var specialStats = uiManager.ItemDetailsUI.transform.GetChild(0);

            if (item.SpecialStats.Attributes != null)
            {
                if (item.SpecialStats.Attributes.Strength != 0)
                {
                    var str = specialStats.GetChild(0).GetChild(0).gameObject;
                    str.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = item.SpecialStats.Attributes.Strength.ToString();
                    str.SetActive(true);
                }
                if (item.SpecialStats.Attributes.Dexterity != 0)
                {
                    var dex = specialStats.GetChild(0).GetChild(1).gameObject;
                    dex.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = item.SpecialStats.Attributes.Dexterity.ToString();
                    dex.SetActive(true);
                }
                if (item.SpecialStats.Attributes.Constitution != 0)
                {
                    var con = specialStats.GetChild(0).GetChild(2).gameObject;
                    con.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = item.SpecialStats.Attributes.Constitution.ToString();
                    con.SetActive(true);
                }
                if (item.SpecialStats.Attributes.Intelligence != 0)
                {
                    var intel = specialStats.GetChild(0).GetChild(3).gameObject;
                    intel.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = item.SpecialStats.Attributes.Intelligence.ToString();
                    intel.SetActive(true);
                }
                if (item.SpecialStats.Attributes.Charisma != 0)
                {
                    var cha = specialStats.GetChild(0).GetChild(4).gameObject;
                    cha.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = item.SpecialStats.Attributes.Charisma.ToString();
                    cha.SetActive(true);
                }
            }

            bool shouldShow = false;
            foreach (var dmgType in item.SpecialStats.Resistances.Keys)
            {
                if (item.SpecialStats.Resistances[dmgType] != 0)
                {
                    shouldShow = true;
                }
            }
            if (item.SpecialStats.Resistances != null && shouldShow)
            {
                var resistances = specialStats.GetChild(1);
                resistances.GetChild(0).GetChild(0).GetComponent<Text>().text = string.Format("{0}%", item.SpecialStats.Resistances.ContainsKey(DamageTypeEnum.Piercing) ? item.SpecialStats.Resistances[DamageTypeEnum.Piercing] : 0);
                resistances.GetChild(1).GetChild(0).GetComponent<Text>().text = string.Format("{0}%", item.SpecialStats.Resistances.ContainsKey(DamageTypeEnum.Slashing) ? item.SpecialStats.Resistances[DamageTypeEnum.Slashing] : 0);
                resistances.GetChild(2).GetChild(0).GetComponent<Text>().text = string.Format("{0}%", item.SpecialStats.Resistances.ContainsKey(DamageTypeEnum.Bludgeoning) ? item.SpecialStats.Resistances[DamageTypeEnum.Bludgeoning] : 0);
                resistances.GetChild(3).GetChild(0).GetComponent<Text>().text = string.Format("{0}%", item.SpecialStats.Resistances.ContainsKey(DamageTypeEnum.Fire) ? item.SpecialStats.Resistances[DamageTypeEnum.Fire] : 0);
                resistances.GetChild(4).GetChild(0).GetComponent<Text>().text = string.Format("{0}%", item.SpecialStats.Resistances.ContainsKey(DamageTypeEnum.Cold) ? item.SpecialStats.Resistances[DamageTypeEnum.Cold] : 0);
                resistances.GetChild(5).GetChild(0).GetComponent<Text>().text = string.Format("{0}%", item.SpecialStats.Resistances.ContainsKey(DamageTypeEnum.Lightning) ? item.SpecialStats.Resistances[DamageTypeEnum.Lightning] : 0);
                resistances.GetChild(6).GetChild(0).GetComponent<Text>().text = string.Format("{0}%", item.SpecialStats.Resistances.ContainsKey(DamageTypeEnum.Poison) ? item.SpecialStats.Resistances[DamageTypeEnum.Poison] : 0);
                resistances.gameObject.SetActive(true);
            }

            shouldShow = false;
            foreach (var dmgType in item.SpecialStats.Damage.Keys)
            {
                if (item.SpecialStats.Damage[dmgType].MinValue != 0 && item.SpecialStats.Damage[dmgType].MaxValue != 0)
                {
                    shouldShow = true;
                }
            }
            if (item.SpecialStats.Damage != null && shouldShow)
            {
                var damages = specialStats.GetChild(2);
                if (item.SpecialStats.Damage.ContainsKey(DamageTypeEnum.Piercing))
                {
                    if (item.SpecialStats.Damage[DamageTypeEnum.Piercing].MinValue > 0 && item.SpecialStats.Damage[DamageTypeEnum.Piercing].MaxValue > 0)
                    {
                        damages.GetChild(0).GetChild(1).GetComponent<Text>().text = string.Format("{0} - {1}", item.SpecialStats.Damage[DamageTypeEnum.Piercing].MinValue, item.SpecialStats.Damage[DamageTypeEnum.Piercing].MaxValue);
                        damages.GetChild(0).gameObject.SetActive(true);
                    }
                }
                if (item.SpecialStats.Damage.ContainsKey(DamageTypeEnum.Slashing))
                {
                    if (item.SpecialStats.Damage[DamageTypeEnum.Slashing].MinValue > 0 && item.SpecialStats.Damage[DamageTypeEnum.Slashing].MaxValue > 0)
                    {
                        damages.GetChild(1).GetChild(1).GetComponent<Text>().text = string.Format("{0} - {1}", item.SpecialStats.Damage[DamageTypeEnum.Slashing].MinValue, item.SpecialStats.Damage[DamageTypeEnum.Slashing].MaxValue);
                        damages.GetChild(1).gameObject.SetActive(true);
                    }
                }
                if (item.SpecialStats.Damage.ContainsKey(DamageTypeEnum.Bludgeoning))
                {
                    if (item.SpecialStats.Damage[DamageTypeEnum.Bludgeoning].MinValue > 0 && item.SpecialStats.Damage[DamageTypeEnum.Bludgeoning].MaxValue > 0)
                    {
                        damages.GetChild(2).GetChild(1).GetComponent<Text>().text = string.Format("{0} - {1}", item.SpecialStats.Damage[DamageTypeEnum.Bludgeoning].MinValue, item.SpecialStats.Damage[DamageTypeEnum.Bludgeoning].MaxValue);
                        damages.GetChild(2).gameObject.SetActive(true);
                    }
                }
                if (item.SpecialStats.Damage.ContainsKey(DamageTypeEnum.Fire))
                {
                    if (item.SpecialStats.Damage[DamageTypeEnum.Fire].MinValue > 0 && item.SpecialStats.Damage[DamageTypeEnum.Fire].MaxValue > 0)
                    {
                        damages.GetChild(3).GetChild(1).GetComponent<Text>().text = string.Format("{0} - {1}", item.SpecialStats.Damage[DamageTypeEnum.Fire].MinValue, item.SpecialStats.Damage[DamageTypeEnum.Fire].MaxValue);
                        damages.GetChild(3).gameObject.SetActive(true);
                    }
                }
                if (item.SpecialStats.Damage.ContainsKey(DamageTypeEnum.Cold))
                {
                    if (item.SpecialStats.Damage[DamageTypeEnum.Cold].MinValue > 0 && item.SpecialStats.Damage[DamageTypeEnum.Cold].MaxValue > 0)
                    {
                        damages.GetChild(4).GetChild(1).GetComponent<Text>().text = string.Format("{0} - {1}", item.SpecialStats.Damage[DamageTypeEnum.Cold].MinValue, item.SpecialStats.Damage[DamageTypeEnum.Cold].MaxValue);
                        damages.GetChild(4).gameObject.SetActive(true);
                    }
                }
                if (item.SpecialStats.Damage.ContainsKey(DamageTypeEnum.Lightning))
                {
                    if (item.SpecialStats.Damage[DamageTypeEnum.Lightning].MinValue > 0 && item.SpecialStats.Damage[DamageTypeEnum.Lightning].MaxValue > 0)
                    {
                        damages.GetChild(5).GetChild(1).GetComponent<Text>().text = string.Format("{0} - {1}", item.SpecialStats.Damage[DamageTypeEnum.Lightning].MinValue, item.SpecialStats.Damage[DamageTypeEnum.Lightning].MaxValue);
                        damages.GetChild(5).gameObject.SetActive(true);
                    }
                }
                if (item.SpecialStats.Damage.ContainsKey(DamageTypeEnum.Poison))
                {
                    if (item.SpecialStats.Damage[DamageTypeEnum.Poison].MinValue > 0 && item.SpecialStats.Damage[DamageTypeEnum.Poison].MaxValue > 0)
                    {
                        damages.GetChild(6).GetChild(1).GetComponent<Text>().text = string.Format("{0} - {1}", item.SpecialStats.Damage[DamageTypeEnum.Poison].MinValue, item.SpecialStats.Damage[DamageTypeEnum.Poison].MaxValue);
                        damages.GetChild(6).gameObject.SetActive(true);
                    }
                }

                damages.gameObject.SetActive(true);
            }
        }
    }

    private void ShowDetails(InventoryItem item)
    {
        Transform quantityDetails;
        Transform durabilityDetails;
        Transform speedDetails;
        Transform inventorySlotsDetails;
        Transform staminaCostDetails;
        Transform attackRecoveryDetails;

        detailsObject.SetActive(true);

        var icon = detailsObject.transform.GetChild(0);
        icon.GetComponent<Image>().sprite = item.GameObject.GetComponent<SpriteRenderer>().sprite;

        var itemNameText = detailsObject.transform.GetChild(1);
        itemNameText.GetComponent<Text>().text = item.Name;

        var itemDescriptionText = detailsObject.transform.GetChild(2);
        itemDescriptionText.GetComponent<Text>().text = item.Description;

        var itemTypeText = detailsObject.transform.GetChild(3);
        itemTypeText.GetComponent<Text>().text = item.Type.ToString();

        switch (item.Type)
        {
            case ItemTypeEnum.Armor:
            case ItemTypeEnum.Head:
            case ItemTypeEnum.Shield:
            case ItemTypeEnum.Hip:
            case ItemTypeEnum.Feet:
            case ItemTypeEnum.Hands:
                durabilityDetails = detailsObject.transform.GetChild(4);
                durabilityDetails.GetChild(1).GetComponent<Text>().text = string.Format("{0}/{1}", item.Durability.Value, item.Durability.MaxValue);

                if (item.Type == ItemTypeEnum.Hip)
                {
                    inventorySlotsDetails = detailsObject.transform.GetChild(6);
                    inventorySlotsDetails.GetChild(1).GetComponent<Text>().text = string.Format("{0}", item.InventorySlots.Value);
                    inventorySlotsDetails.gameObject.SetActive(true);
                }

                if (item.Type == ItemTypeEnum.Hands)
                {
                    var critChance = detailsObject.transform.GetChild(8);
                    critChance.GetChild(1).GetComponent<Text>().text = string.Format("{0}", item.CriticalChance.Value);
                    critChance.gameObject.SetActive(true);
                }

                if (item.Type == ItemTypeEnum.Feet)
                {
                    speedDetails = detailsObject.transform.GetChild(7);
                    speedDetails.GetChild(1).GetComponent<Text>().text = string.Format("{0}", item.Speed.Value);
                    speedDetails.gameObject.SetActive(true);
                }

                if (item.Type != ItemTypeEnum.Hip)
                {
                    ShowResistanceDetails(item);
                }

                ShowSpecialStats(item);
                break;
            case ItemTypeEnum.Ammo:
                quantityDetails = detailsObject.transform.GetChild(4);
                quantityDetails.GetChild(1).GetComponent<Text>().text = string.Format("{0}/{1}", item.Quantity.Value, item.Quantity.MaxValue);

                ShowSpecialStats(item);
                break;
            case ItemTypeEnum.Consumable:
                quantityDetails = detailsObject.transform.GetChild(4);
                quantityDetails.GetChild(1).GetComponent<Text>().text = string.Format("{0}/{1}", item.Quantity.Value, item.Quantity.MaxValue);

                var regainDetails = detailsObject.transform.GetChild(5);
                regainDetails.GetChild(0).GetComponent<Text>().text = item.Regain.MinValue.ToString();
                regainDetails.GetChild(1).GetComponent<Text>().text = item.Regain.MaxValue.ToString();

                ShowSpecialStats(item);
                break;
            case ItemTypeEnum.Neck:
            case ItemTypeEnum.Ring:
                ShowSpecialStats(item);
                break;
            case ItemTypeEnum.Weapon:
                durabilityDetails = detailsObject.transform.GetChild(4);
                durabilityDetails.GetChild(1).GetComponent<Text>().text = string.Format("{0}/{1}", item.Durability.Value, item.Durability.MaxValue);

                attackRecoveryDetails = detailsObject.transform.GetChild(5);
                attackRecoveryDetails.GetChild(1).GetComponent<Text>().text = string.Format("{0}", item.AttackRecoveryTime.Value);

                staminaCostDetails = detailsObject.transform.GetChild(6);
                staminaCostDetails.GetChild(1).GetComponent<Text>().text = string.Format("{0}", item.StaminaConsumption.Value);

                // TODO: Show Charge Speed

                ShowDamageDetails(item);

                ShowSpecialStats(item);
                break;
            default:
                break;
        }
    }

    private void ShowResistanceDetails(InventoryItem item)
    {
        Transform resistanceDetails = detailsObject.transform.GetChild(detailsObject.transform.childCount - 2);
        var index = 0;
        foreach (var dmgType in item.Resistance.Keys)
        {
            var resDetails = resistanceDetails.GetChild(index);
            var resIcon = resDetails.GetChild(0).GetComponent<Image>();
            switch (dmgType)
            {
                case DamageTypeEnum.Bludgeoning:
                    resIcon.sprite = uiManager.BludgeoningIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Piercing:
                    resIcon.sprite = uiManager.PiercingIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Slashing:
                    resIcon.sprite = uiManager.SlashingIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Fire:
                    resIcon.sprite = uiManager.FireIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Cold:
                    resIcon.sprite = uiManager.ColdIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Lightning:
                    resIcon.sprite = uiManager.LightningIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Poison:
                    resIcon.sprite = uiManager.PoisonIcon.GetComponent<Image>().sprite;
                    break;
            };
            var text = resistanceDetails.GetChild(index).GetChild(1).GetComponent<Text>();
            text.text = string.Format("{0}", item.Resistance[dmgType].Value);
            index++;
            resDetails.gameObject.SetActive(true);
        }
    }

    private void ShowDamageDetails(InventoryItem item)
    {
        Transform damageDetails = detailsObject.transform.GetChild(detailsObject.transform.childCount - 2);
        var index = 0;
        foreach (var dmgType in item.Damage.Keys)
        {
            var dmgDetails = damageDetails.GetChild(index);
            var dmgIcon = dmgDetails.GetChild(0).GetComponent<Image>();
            switch (dmgType)
            {
                case DamageTypeEnum.Bludgeoning:
                    dmgIcon.sprite = uiManager.BludgeoningIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Piercing:
                    dmgIcon.sprite = uiManager.PiercingIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Slashing:
                    dmgIcon.sprite = uiManager.SlashingIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Fire:
                    dmgIcon.sprite = uiManager.FireIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Cold:
                    dmgIcon.sprite = uiManager.ColdIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Lightning:
                    dmgIcon.sprite = uiManager.LightningIcon.GetComponent<Image>().sprite;
                    break;
                case DamageTypeEnum.Poison:
                    dmgIcon.sprite = uiManager.PoisonIcon.GetComponent<Image>().sprite;
                    break;
            };
            var text = damageDetails.GetChild(index).GetChild(1).GetComponent<Text>();
            text.text = string.Format("{0} - {1}", item.Damage[dmgType].MinValue, item.Damage[dmgType].MaxValue);
            index++;
            dmgDetails.gameObject.SetActive(true);
        }
    }
}