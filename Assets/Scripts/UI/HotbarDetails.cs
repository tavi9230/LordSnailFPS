using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotbarDetails : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UIManager uiManager;
    private PlayerController playerController;
    private HotbarItem hotbarItem;
    private GameManager gameManager;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (hotbarItem != null)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (hotbarItem.Skill != null)
                {
                    playerController.Stats.LeftHandAttack.Attack.Replace(hotbarItem.Skill);
                    uiManager.RefreshInventory();
                }
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                if (hotbarItem.Skill != null)
                {
                    playerController.Stats.RightHandAttack.Attack.Replace(hotbarItem.Skill);
                    uiManager.RefreshInventory();
                }
            }
        }

        // can't use hotbar items while looking in menu
        if (!uiManager.PlayerInfoUI.activeSelf)
        {
            // we check for the index because the script is attached to all 10 hotbar items
            if (Input.GetKeyDown(KeyCode.Alpha0) && gameObject.transform.parent.GetSiblingIndex() == 9)
            {
                SelectHotbarItem(KeyCode.Alpha0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) && gameObject.transform.parent.GetSiblingIndex() == 0)
            {
                SelectHotbarItem(KeyCode.Alpha1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && gameObject.transform.parent.GetSiblingIndex() == 1)
            {
                SelectHotbarItem(KeyCode.Alpha2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && gameObject.transform.parent.GetSiblingIndex() == 2)
            {
                SelectHotbarItem(KeyCode.Alpha3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && gameObject.transform.parent.GetSiblingIndex() == 3)
            {
                SelectHotbarItem(KeyCode.Alpha4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) && gameObject.transform.parent.GetSiblingIndex() == 4)
            {
                SelectHotbarItem(KeyCode.Alpha5);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6) && gameObject.transform.parent.GetSiblingIndex() == 5)
            {
                SelectHotbarItem(KeyCode.Alpha6);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7) && gameObject.transform.parent.GetSiblingIndex() == 6)
            {
                SelectHotbarItem(KeyCode.Alpha7);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8) && gameObject.transform.parent.GetSiblingIndex() == 7)
            {
                SelectHotbarItem(KeyCode.Alpha8);
            }
            if (Input.GetKeyDown(KeyCode.Alpha9) && gameObject.transform.parent.GetSiblingIndex() == 8)
            {
                SelectHotbarItem(KeyCode.Alpha9);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (hotbarItem != null && hotbarItem.Item != null && hotbarItem.Item.Type == ItemTypeEnum.Consumable)
            {
                var index = transform.parent.GetSiblingIndex();
                SelectHotbarItem(GetHoveredKeyCode(index));
            }
        }
    }

    private void SelectHotbarItem(KeyCode keyCode)
    {
        if (playerController.Hotbar[keyCode].Skill != null && playerController.Hotbar[keyCode].Skill.Type == SkillTypeEnum.Active)
        {
            playerController.Stats.ActiveSkill = playerController.Hotbar[keyCode].Skill;
        }
        if (playerController.Hotbar[keyCode].Item != null && playerController.Hotbar[keyCode].Item.Type == ItemTypeEnum.Consumable)
        {
            Helpers.ConsumePotion(playerController, uiManager, keyCode);
        }
    }

    private KeyCode GetHoveredKeyCode(int index)
    {
        switch (index)
        {
            case 0:
                return KeyCode.Alpha1;
            case 1:
                return KeyCode.Alpha2;
            case 2:
                return KeyCode.Alpha3;
            case 3:
                return KeyCode.Alpha4;
            case 4:
                return KeyCode.Alpha5;
            case 5:
                return KeyCode.Alpha6;
            case 6:
                return KeyCode.Alpha7;
            case 7:
                return KeyCode.Alpha8;
            case 8:
                return KeyCode.Alpha9;
            case 9:
                return KeyCode.Alpha0;
            default:
                return KeyCode.Alpha1;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var size = uiManager.SkillDetailsUI.transform.GetComponent<RectTransform>().sizeDelta;
        var index = transform.parent.GetSiblingIndex();
        HotbarItem hb = playerController.Hotbar[GetHoveredKeyCode(index)];

        // TODO if hotbar item is skill:
        if (hb.Skill != null)
        {
            ShowDetails(hb.Skill);
            uiManager.SkillDetailsUI.SetActive(true);
        }

        // TODO if hotbar item is item:
        if (hb.Item != null)
        {
            ShowDetails(hb.Item);
            uiManager.ItemDetailsUI.SetActive(true);
        }

        hotbarItem = hb;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideAllDetails();
        hotbarItem = null;
    }

    public void HideAllDetails()
    {
        uiManager.SkillDetailsUI.SetActive(false);
        uiManager.ItemDetailsUI.SetActive(false);
    }

    private void ShowDetails(InventoryItem item)
    {
        ItemDetails itemDetails = gameManager.GetComponent<ItemDetails>();
        itemDetails.SelectDetailsObjectAndShowItemDetails(item);
    }

    private void ShowDetails(Skill skill)
    {
        var icon = uiManager.SkillDetailsUI.transform.GetChild(0);
        icon.GetComponent<Image>().sprite = skill.GameObject.GetComponent<Image>().sprite;

        var skillNameText = uiManager.SkillDetailsUI.transform.GetChild(1);
        skillNameText.GetComponent<Text>().text = skill.Name;

        var skillDescriptionText = uiManager.SkillDetailsUI.transform.GetChild(2);
        skillDescriptionText.GetComponent<Text>().text = skill.Description;

        var skillTypeText = uiManager.SkillDetailsUI.transform.GetChild(3);
        skillTypeText.GetComponent<Text>().text = skill.Type.ToString();

        var attackRecoveryText = uiManager.SkillDetailsUI.transform.GetChild(4).GetChild(1);
        attackRecoveryText.GetComponent<Text>().text = skill.AttackRecoveryTime.ToString();

        var manaCost = uiManager.SkillDetailsUI.transform.GetChild(5).GetChild(1);
        manaCost.GetComponent<Text>().text = skill.ManaConsumption.ToString();

        ShowDamageDetails(skill);
    }

    private void ShowDamageDetails(Skill skill)
    {
        Transform damageDetails = uiManager.SkillDetailsUI.transform.GetChild(uiManager.SkillDetailsUI.transform.childCount - 1);
        var index = 0;
        foreach (var dmgType in skill.Damage.Keys)
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
            text.text = string.Format("{0} - {1}", skill.Damage[dmgType].MinValue, skill.Damage[dmgType].MaxValue);
            index++;
            dmgDetails.gameObject.SetActive(true);
        }
    }

    //private void ShowResistanceDetails(Skill skill)
    //{
    //    Transform resistanceDetails = uiManager.SpellDetailsUI.transform.GetChild(uiManager.SpellDetailsUI.transform.childCount - 1);
    //    var index = 0;
    //    foreach (var dmgType in skill.Resistance.Keys)
    //    {
    //        var resDetails = resistanceDetails.GetChild(index);
    //        var resIcon = resDetails.GetChild(0).GetComponent<Image>();
    //        switch (dmgType)
    //        {
    //            case DamageTypeEnum.Bludgeoning:
    //                resIcon.sprite = uiManager.BludgeoningIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Piercing:
    //                resIcon.sprite = uiManager.PiercingIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Slashing:
    //                resIcon.sprite = uiManager.SlashingIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Fire:
    //                resIcon.sprite = uiManager.FireIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Cold:
    //                resIcon.sprite = uiManager.ColdIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Lightning:
    //                resIcon.sprite = uiManager.LightningIcon.GetComponent<Image>().sprite;
    //                break;
    //            case DamageTypeEnum.Poison:
    //                resIcon.sprite = uiManager.PoisonIcon.GetComponent<Image>().sprite;
    //                break;
    //        };
    //        var text = resistanceDetails.GetChild(index).GetChild(1).GetComponent<Text>();
    //        text.text = string.Format("{0}", skill.Resistance[dmgType].Value);
    //        index++;
    //        resDetails.gameObject.SetActive(true);
    //    }
    //}
}