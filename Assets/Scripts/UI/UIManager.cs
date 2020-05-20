using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables

    public Text leftHandText;
    public Text rightHandText;
    public Sprite LockSprite;
    public GameObject Tooltip;
    public GameObject ObjectInventoryUI;
    public GameObject InventorySlot;
    public GameObject ObjectInventorySlot;
    public GameObject PlayerInfoUI;
    public GameObject MainBoardUI;
    public GameObject ItemDetailsUI;
    public GameObject EnemyInfoUI;
    public GameObject PlayerAttackPowerUI;
    public GameObject PlayerSkillsUI;
    public GameObject SkillDetailsUI;

    public GameObject PiercingIcon;
    public GameObject SlashingIcon;
    public GameObject BludgeoningIcon;
    public GameObject FireIcon;
    public GameObject ColdIcon;
    public GameObject LightningIcon;
    public GameObject PoisonIcon;

    public GameObject InspectedEnemy;

    private HealthManager healthManager;
    private PlayerController playerController;
    private InventoryManager inventoryManager;
    private GameManager gameManager;

    private GameObject InventoryUI;
    private GameObject PlayerAttributesUI;
    private GameObject PlayerResistancesUI;
    #endregion

    private void Awake()
    {
        MainBoardUI.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        healthManager = playerController.gameObject.GetComponent<HealthManager>();
        inventoryManager = playerController.gameObject.GetComponent<InventoryManager>();
        gameManager = FindObjectOfType<GameManager>();
        InventoryUI = PlayerInfoUI.transform.GetChild(0).gameObject;
        PlayerAttributesUI = PlayerInfoUI.transform.GetChild(1).gameObject;
        PlayerResistancesUI = PlayerInfoUI.transform.GetChild(2).gameObject;
        PlayerAttackPowerUI = PlayerInfoUI.transform.GetChild(3).gameObject;
        PlayerSkillsUI = PlayerInfoUI.transform.GetChild(4).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Transform health = MainBoardUI.transform.GetChild(0);
        Transform mana = MainBoardUI.transform.GetChild(1);
        Transform stamina = MainBoardUI.transform.GetChild(2);
        Transform leftHand = MainBoardUI.transform.GetChild(3);
        Transform rightHand = MainBoardUI.transform.GetChild(4);
        health.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0:0.0}/{1:0.0}", decimal.Round((decimal)playerController.Stats.TotalHealth, 1), decimal.Round((decimal)playerController.Stats.TotalMaxHealth, 1));
        mana.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0:0.0}/{1:0.0}", decimal.Round((decimal)playerController.Stats.TotalMana, 1), decimal.Round((decimal)playerController.Stats.TotalMaxMana, 1));
        stamina.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0:0.0}/{1:0.0}", decimal.Round((decimal)playerController.Stats.TotalStamina, 1), decimal.Round((decimal)playerController.Stats.TotalMaxStamina, 1));

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (PlayerSkillsUI.activeSelf == true)
            {
                ToggleSkillsInfo();
            }
            ToggleInventory();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (InventoryUI.activeSelf == true)
            {
                ToggleInventory();
            }
            ToggleSkillsInfo();
        }

        ShowActiveHandSlots(leftHand, playerController.Stats.LeftHandAttack);
        ShowActiveHandSlots(rightHand, playerController.Stats.RightHandAttack);
    }

    private void ShowActiveHandSlots(Transform hand, AttackPower attackPower)
    {
        if (attackPower.Attack.Item != null)
        {
            if (attackPower.Attack.Item.Type != ItemTypeEnum.None)
            {
                hand.GetChild(0).GetChild(0).GetComponent<Image>().sprite = attackPower.Attack.Item.GameObject.GetComponent<SpriteRenderer>().sprite;
                hand.GetChild(0).GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                hand.GetChild(0).GetChild(0).GetComponent<Image>().sprite = null;
                hand.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }

            if (attackPower.Attack.Item.Type == ItemTypeEnum.Ammo
                || attackPower.Attack.Item.Type == ItemTypeEnum.Consumable)
            {
                hand.GetChild(1).GetComponent<Text>().text = string.Format("{0}/{1}", attackPower.Attack.Item.Quantity.Value, attackPower.Attack.Item.Quantity.MaxValue);
                hand.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                hand.GetChild(1).gameObject.SetActive(false);
            }
        }
        else if (attackPower.Attack.Skill != null)
        {
            hand.GetChild(0).GetChild(0).GetComponent<Image>().sprite = attackPower.Attack.Skill.GameObject.GetComponent<Image>().sprite;
            hand.GetChild(0).GetChild(0).gameObject.SetActive(true);
            hand.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            hand.GetChild(0).GetChild(0).gameObject.SetActive(false);
            hand.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void PopulateHotbar()
    {
        foreach (var key in playerController.Hotbar.Keys)
        {
            int index = 0;
            switch (key)
            {
                case KeyCode.Alpha0:
                    index = 9;
                    break;
                case KeyCode.Alpha1:
                    index = 0;
                    break;
                case KeyCode.Alpha2:
                    index = 1;
                    break;
                case KeyCode.Alpha3:
                    index = 2;
                    break;
                case KeyCode.Alpha4:
                    index = 3;
                    break;
                case KeyCode.Alpha5:
                    index = 4;
                    break;
                case KeyCode.Alpha6:
                    index = 5;
                    break;
                case KeyCode.Alpha7:
                    index = 6;
                    break;
                case KeyCode.Alpha8:
                    index = 7;
                    break;
                case KeyCode.Alpha9:
                    index = 8;
                    break;
                default:
                    index = 0;
                    break;
            }
            var hotbarItem = MainBoardUI.transform.GetChild(5).GetChild(index);
            var icon = hotbarItem.GetChild(0).GetChild(0);

            if (playerController.Hotbar[key].Skill != null)
            {
                icon.GetComponent<Image>().sprite = playerController.Hotbar[key].Skill.GameObject.GetComponent<Image>().sprite;
                icon.gameObject.SetActive(true);
            }
            else if (playerController.Hotbar[key].Item != null)
            {
                icon.GetComponent<Image>().sprite = playerController.Hotbar[key].Item.GameObject.GetComponent<SpriteRenderer>().sprite;
                icon.gameObject.SetActive(true);

                if (playerController.Hotbar[key].Item.Type == ItemTypeEnum.Consumable)
                {
                    hotbarItem.GetChild(1).GetComponent<Text>().text = playerController.Hotbar[key].Item.Quantity.ToString();
                }
            }
        }
    }

    public void ResetHotbar()
    {
        foreach (var key in playerController.Hotbar.Keys)
        {
            int index = 0;
            switch (key)
            {
                case KeyCode.Alpha0:
                    index = 0;
                    break;
                case KeyCode.Alpha1:
                    index = 1;
                    break;
                case KeyCode.Alpha2:
                    index = 2;
                    break;
                case KeyCode.Alpha3:
                    index = 3;
                    break;
                case KeyCode.Alpha4:
                    index = 4;
                    break;
                case KeyCode.Alpha5:
                    index = 5;
                    break;
                case KeyCode.Alpha6:
                    index = 6;
                    break;
                case KeyCode.Alpha7:
                    index = 7;
                    break;
                case KeyCode.Alpha8:
                    index = 8;
                    break;
                case KeyCode.Alpha9:
                    index = 9;
                    break;
                default:
                    index = 0;
                    break;
            }
            var hotbarItem = MainBoardUI.transform.GetChild(5).GetChild(index);
            hotbarItem.GetChild(0).GetChild(0).gameObject.SetActive(false);
            hotbarItem.GetChild(1).gameObject.SetActive(false);
        }
        PopulateHotbar();
    }

    private void ToggleSkillsInfo()
    {
        if (PlayerSkillsUI.activeSelf == true)
        {
            PlayerInfoUI.SetActive(false);
            PlayerSkillsUI.SetActive(false);
            SkillDetailsUI.SetActive(false);
            for (var i = 0; i < PlayerSkillsUI.transform.GetChild(1).childCount; i++)
            {
                Destroy(PlayerSkillsUI.transform.GetChild(1).GetChild(i).gameObject);
            }
            for (var i = 0; i < PlayerSkillsUI.transform.GetChild(2).childCount; i++)
            {
                Destroy(PlayerSkillsUI.transform.GetChild(2).GetChild(i).gameObject);
            }
        }
        else
        {
            PlayerInfoUI.SetActive(true);
            PlayerSkillsUI.SetActive(true);

            PlayerSkillsUI.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = playerController.Stats.StoredExperiencePoints.ToString();

            foreach (Skill inactiveSkill in playerController.Stats.Skills[ActivityEnum.Inactive])
            {
                GameObject skill = Instantiate((GameObject)Resources.Load("Prefabs/UI/SkillSlot"), PlayerSkillsUI.transform.GetChild(1).transform);
                skill.transform.GetChild(0).GetComponent<SkillStatus>().SetSkill(inactiveSkill);

                var icon = skill.transform.GetChild(0).GetChild(0);
                icon.GetComponent<Image>().sprite = inactiveSkill.GameObject.GetComponent<Image>().sprite;

                if (inactiveSkill.IsUnlocked)
                {
                    skill.transform.GetChild(0).GetChild(1).GetComponent<Image>().gameObject.SetActive(false);
                }
            }

            foreach (Skill activeSkill in playerController.Stats.Skills[ActivityEnum.Active])
            {
                GameObject skill = Instantiate((GameObject)Resources.Load("Prefabs/UI/ActiveSkillSlot"), PlayerSkillsUI.transform.GetChild(2).transform);
                skill.transform.GetChild(0).GetComponent<SkillStatus>().SetSkill(activeSkill);

                var icon = skill.transform.GetChild(0).GetChild(0);
                icon.GetComponent<Image>().sprite = activeSkill.GameObject.GetComponent<Image>().sprite;
            }
        }
    }

    public bool CanAttack()
    {
        return InventoryUI.activeSelf != true && PlayerSkillsUI.activeSelf != true;
    }

    public void RefreshInventory(GameObject owner = null, InventoryItem item = null)
    {
        if (InventoryUI.activeSelf == true)
        {
            ToggleInventory();
            ToggleInventory();
        }

        if (PlayerSkillsUI.activeSelf == true)
        {
            ToggleSkillsInfo();
            ToggleSkillsInfo();
        }

        if (ObjectInventoryUI.activeSelf == true && owner != null)
        {
            CloseObjectInventory();
            owner.GetComponent<ObjectController>().DisplayObjectInventory();
        }

        if (EnemyInfoUI.activeSelf == true && owner != null && owner.CompareTag("Enemy"))
        {
            CloseEnemyInfo();
            OpenEnemyInventory(owner.GetComponent<EnemyController>().InventoryManager, owner);
        }

        var itemDetails = gameManager.GetComponent<ItemDetails>();
        if (ItemDetailsUI.activeSelf)
        {
            itemDetails.RefreshDetails(item);
        }
        
        if (SkillDetailsUI.activeSelf)
        {
            //Refresh SkillDetails
            SkillDetailsUI.SetActive(false);
        }
    }

    public void ToggleInventory()
    {
        if (!InventoryUI.activeSelf)
        {
            OpenInventory();
        }
        else
        {
            CloseInventory();
        }
    }

    private void OpenInventory()
    {
        PopulateAttributes(PlayerAttributesUI, playerController.Stats.TotalAttributes);
        PopulateResistances(PlayerResistancesUI, playerController.Stats.TotalResistances);
        PopulatePlayerAttackPower(PlayerAttackPowerUI, playerController.Stats);
        PopulateBodyInventory(InventoryUI, playerController.InventoryManager, playerController.gameObject);
        PopulateInventoryItems(InventoryUI, playerController.InventoryManager, playerController.gameObject, playerController.Stats.MaxInventorySlots);
        PlayerInfoUI.SetActive(true);
        InventoryUI.SetActive(true);
        PlayerAttributesUI.SetActive(true);
        PlayerResistancesUI.SetActive(true);
        PlayerAttackPowerUI.SetActive(true);
        // TODO: Show attack recovery time, stamina recovery time, health recovery time, mana recovery time, stamina consumption
    }

    public void CloseInventory()
    {
        ItemDetailsUI.SetActive(false);
        SkillDetailsUI.SetActive(false);
        ClosePlayerInfo();
        CloseObjectInventory();
    }

    private void ClosePlayerInfo()
    {
        for (var i = 0; i < PlayerInfoUI.transform.childCount; i++)
        {
            PlayerInfoUI.transform.GetChild(i).gameObject.SetActive(false);
        }
        PlayerInfoUI.SetActive(false);
    }

    public void CloseObjectInventory()
    {
        var slots = ObjectInventoryUI.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;

        ObjectInventoryUI.gameObject.SetActive(false);
        for (var i = 0; i < slots.transform.childCount; i++)
        {
            var slot = slots.transform.GetChild(i);
            var button = slot.transform.GetChild(0).gameObject;
            var icon = button.transform.GetChild(0).GetComponent<Image>();
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0);
            var btnImg = button.GetComponent<Image>();
            btnImg.color = new Color(btnImg.color.r, btnImg.color.g, btnImg.color.b, 0);
            var removeBtn = slot.transform.GetChild(1).gameObject;
            removeBtn.GetComponent<Button>().interactable = false;
        }

        var ec = ObjectInventoryUI.transform.GetChild(0).gameObject;
        for (var i = 0; i < ec.transform.childCount; i++)
        {
            var slot = ec.transform.GetChild(i);
            var button = slot.transform.GetChild(0).gameObject;
            var icon = button.transform.GetChild(0).GetComponent<Image>();
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0);
            var btnImg = button.GetComponent<Image>();
            btnImg.color = new Color(btnImg.color.r, btnImg.color.g, btnImg.color.b, 0);
            var removeBtn = slot.transform.GetChild(1).gameObject;
            removeBtn.GetComponent<Button>().interactable = false;
        }
    }

    private string GetDamageText(Stats stats, AttackPower attackPower, int i, bool isSkill = false)
    {
        float minValue = 0;
        float maxValue = 0;
        DamageTypeEnum dmgType = (DamageTypeEnum)i;

        if (isSkill)
        {
            if (attackPower.Attack.Skill.Damage[dmgType].MaxValue != 0)
            {
                minValue = attackPower.Attack.Skill.Damage[dmgType].MinValue + ((float)stats.TotalAttributes.Intelligence / 2);
                maxValue = attackPower.Attack.Skill.Damage[dmgType].MaxValue + ((float)stats.TotalAttributes.Intelligence / 2);
            }
        }
        else
        {
            minValue = attackPower.Damage[dmgType].MinValue;
            maxValue = attackPower.Damage[dmgType].MaxValue;
        }

        return string.Format("{0} - {1}", minValue, maxValue);
    }

    private void PopulatePlayerAttackPower(GameObject attackPowerParent, Stats stats)
    {
        // Left Hand
        Transform leftHand = attackPowerParent.transform.GetChild(1);
        Transform rightHand = attackPowerParent.transform.GetChild(2);
        if (stats.LeftHandAttack.Attack.Item != null)
        {
            for (var i = 0; i < leftHand.GetChild(2).childCount; i++)
            {
                string text = GetDamageText(stats, stats.LeftHandAttack, i);
                leftHand.GetChild(2).GetChild(i).GetComponent<Text>().text = text;
            }

            leftHand.GetChild(3).GetChild(1).GetComponent<Text>().text = string.Format("{0}", stats.LeftHandAttack.AttackRecoveryTime.ToString());
            leftHand.GetChild(4).GetChild(1).GetComponent<Text>().text = string.Format("{0}", stats.LeftHandAttack.StaminaConsumption.ToString());
        }
        else if (stats.LeftHandAttack.Attack.Skill != null)
        {
            for (var i = 0; i < leftHand.GetChild(2).childCount; i++)
            {
                string text = GetDamageText(stats, stats.LeftHandAttack, i, true);
                leftHand.GetChild(2).GetChild(i).GetComponent<Text>().text = text;
            }

            leftHand.GetChild(3).GetChild(1).GetComponent<Text>().text = string.Format("{0}", stats.LeftHandAttack.Attack.Skill.AttackRecoveryTime.ToString());
            leftHand.GetChild(4).GetChild(1).GetComponent<Text>().text = string.Format("{0}", stats.LeftHandAttack.Attack.Skill.ManaConsumption.ToString());
        }

        // Right hand
        if (stats.RightHandAttack.Attack.Item != null)
        {
            for (var i = 0; i < rightHand.transform.GetChild(2).childCount; i++)
            {
                string text = GetDamageText(stats, stats.RightHandAttack, i);
                rightHand.GetChild(2).GetChild(i).GetComponent<Text>().text = text;
            }

            rightHand.GetChild(3).GetChild(1).GetComponent<Text>().text = string.Format("{0}", stats.RightHandAttack.AttackRecoveryTime.ToString());
            rightHand.GetChild(4).GetChild(1).GetComponent<Text>().text = string.Format("{0}", stats.RightHandAttack.StaminaConsumption.ToString());
        }
        else if (stats.RightHandAttack.Attack.Skill != null)
        {
            for (var i = 0; i < rightHand.GetChild(2).childCount; i++)
            {
                string text = GetDamageText(stats, stats.RightHandAttack, i, true);
                rightHand.GetChild(2).GetChild(i).GetComponent<Text>().text = text;
            }

            rightHand.GetChild(3).GetChild(1).GetComponent<Text>().text = string.Format("{0}", stats.RightHandAttack.Attack.Skill.AttackRecoveryTime.ToString());
            rightHand.GetChild(4).GetChild(1).GetComponent<Text>().text = string.Format("{0}", stats.RightHandAttack.Attack.Skill.ManaConsumption.ToString());
        }

        attackPowerParent.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = stats.TotalCriticalChance.ToString();
        attackPowerParent.transform.GetChild(4).GetChild(1).GetComponent<Text>().text = stats.TotalSpeed.ToString();
    }

    private void PopulateResistances(GameObject resistancesParent, Dictionary<DamageTypeEnum, float> resistances)
    {
        for (var i = 0; i < resistancesParent.transform.childCount / 2; i++)
        {
            string text = "";
            switch (i)
            {
                case 0:
                    text = string.Format("{0}%", resistances[DamageTypeEnum.Piercing].ToString());
                    break;
                case 1:
                    text = string.Format("{0}%", resistances[DamageTypeEnum.Slashing].ToString());
                    break;
                case 2:
                    text = string.Format("{0}%", resistances[DamageTypeEnum.Bludgeoning].ToString());
                    break;
                case 3:
                    text = string.Format("{0}%", resistances[DamageTypeEnum.Fire].ToString());
                    break;
                case 4:
                    text = string.Format("{0}%", resistances[DamageTypeEnum.Cold].ToString());
                    break;
                case 5:
                    text = string.Format("{0}%", resistances[DamageTypeEnum.Lightning].ToString());
                    break;
                case 6:
                    text = string.Format("{0}%", resistances[DamageTypeEnum.Poison].ToString());
                    break;
                default:
                    break;
            }
            resistancesParent.transform.GetChild(i).GetComponent<Text>().text = text;
        }
    }

    private void PopulateAttributes(GameObject attributesParent, Attributes attributes)
    {
        bool isEnemy = attributesParent.transform.parent.name.Contains("Enemy");
        if (!isEnemy)
        {
            var totalAttributePoints = attributesParent.transform.GetChild(5);
            totalAttributePoints.GetChild(0).GetComponent<Text>().text = playerController.Stats.AvailableAttributePoints.ToString();
            totalAttributePoints.gameObject.SetActive(playerController.Stats.AvailableAttributePoints > 0);
        }

        for (var i = 0; i < attributesParent.transform.childCount; i++)
        {
            if (i < 5)
            {
                bool isPlusActive = false; ;

                var textObj = attributesParent.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>();
                switch (i)
                {
                    case 0:
                        textObj.text = string.Format("{0}", attributes.Strength.ToString());
                        isPlusActive = attributes.Strength < Constants.ATTRIBUTES_MAX_VALUE && playerController.Stats.AvailableAttributePoints > 0 && !isEnemy;
                        attributesParent.transform.GetChild(i).GetChild(1).gameObject.SetActive(isPlusActive);
                        break;
                    case 1:
                        textObj.text = string.Format("{0}", attributes.Dexterity.ToString());
                        isPlusActive = attributes.Dexterity < Constants.ATTRIBUTES_MAX_VALUE && playerController.Stats.AvailableAttributePoints > 0 && !isEnemy;
                        attributesParent.transform.GetChild(i).GetChild(1).gameObject.SetActive(isPlusActive);
                        break;
                    case 2:
                        textObj.text = string.Format("{0}", attributes.Constitution.ToString());
                        isPlusActive = attributes.Constitution < Constants.ATTRIBUTES_MAX_VALUE && playerController.Stats.AvailableAttributePoints > 0 && !isEnemy;
                        attributesParent.transform.GetChild(i).GetChild(1).gameObject.SetActive(isPlusActive);
                        break;
                    case 3:
                        textObj.text = string.Format("{0}", attributes.Intelligence.ToString());
                        isPlusActive = attributes.Intelligence < Constants.ATTRIBUTES_MAX_VALUE && playerController.Stats.AvailableAttributePoints > 0 && !isEnemy;
                        attributesParent.transform.GetChild(i).GetChild(1).gameObject.SetActive(isPlusActive);
                        break;
                    case 4:
                        textObj.text = string.Format("{0}", attributes.Charisma.ToString());
                        isPlusActive = attributes.Charisma < Constants.ATTRIBUTES_MAX_VALUE && playerController.Stats.AvailableAttributePoints > 0 && !isEnemy;
                        attributesParent.transform.GetChild(i).GetChild(1).gameObject.SetActive(isPlusActive);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void DisplayObjectInventory(string name, List<InventoryItem> content, List<GameObject> enemyContent, GameObject owner)
    {
        var slots = ObjectInventoryUI.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
        var title = ObjectInventoryUI.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>();
        title.text = name;

        for (var i = 0; i < content.Count; i++)
        {
            var slot = slots.transform.GetChild(i).gameObject;
            var button = slot.transform.GetChild(0).gameObject;

            var itemStatus = button.GetComponent<ItemStatus>();
            itemStatus.SetInvetoryItem(InventoryLocationEnum.ObjectInventory, i, content[i], owner);

            var icon = button.transform.GetChild(0).GetComponent<Image>();
            icon.sprite = content[i].GameObject.GetComponent<SpriteRenderer>().sprite;
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 255);

            var btnImg = button.GetComponent<Image>();
            btnImg.color = new Color(btnImg.color.r, btnImg.color.g, btnImg.color.b, 255);

            var removeBtn = slot.transform.GetChild(1).gameObject;
            removeBtn.GetComponent<Button>().interactable = true;
        }

        for (var i = 0; i < enemyContent.Count; i++)
        {
            var ec = ObjectInventoryUI.transform.GetChild(0).gameObject;
            var icon = ec.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
            var button = ec.transform.GetChild(i).transform.GetChild(0);
            var itemStatus = button.GetComponent<ItemStatus>();

            itemStatus.SetInvetoryItem(InventoryLocationEnum.ObjectEnemyInventory, i, new InventoryItem(), owner);
            itemStatus.SetEnemyItem(enemyContent[i]);
            //var enemyController = enemyContent[i].GetComponent<EnemyController>();

            icon.sprite = enemyContent[i].GetComponent<SpriteRenderer>().sprite;
            //icon.sprite = enemyController.DisplaySprite;
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 255);

            var btnImg = button.GetComponent<Image>();
            btnImg.color = new Color(btnImg.color.r, btnImg.color.g, btnImg.color.b, 255);

            var removeBtn = ec.transform.GetChild(i).transform.GetChild(1).gameObject;
            removeBtn.GetComponent<Button>().interactable = true;
        }

        OpenInventory();
        ObjectInventoryUI.gameObject.SetActive(true);
    }

    public void OpenEnemyInventory(InventoryManager inventory, GameObject owner)
    {
        GameObject inventoryUI = EnemyInfoUI.transform.GetChild(0).gameObject;
        GameObject attributesUI = EnemyInfoUI.transform.GetChild(1).gameObject;
        GameObject resistancesUI = EnemyInfoUI.transform.GetChild(2).gameObject;
        GameObject attackPowerUI = EnemyInfoUI.transform.GetChild(3).gameObject;
        PopulateAttributes(attributesUI, InspectedEnemy.GetComponent<EnemyController>().Stats.TotalAttributes);
        PopulateResistances(resistancesUI, InspectedEnemy.GetComponent<EnemyController>().Stats.TotalResistances);
        PopulatePlayerAttackPower(attackPowerUI, InspectedEnemy.GetComponent<EnemyController>().Stats);
        PopulateBodyInventory(inventoryUI, inventory, owner);
        PopulateInventoryItems(inventoryUI, inventory, owner, owner.GetComponent<EnemyController>().Stats.MaxInventorySlots);
        EnemyInfoUI.SetActive(true);
        inventoryUI.SetActive(true);
        attributesUI.SetActive(true);
        resistancesUI.SetActive(true);
        attackPowerUI.SetActive(true);
    }

    public void CloseEnemyInfo()
    {
        for (var i = 0; i < EnemyInfoUI.transform.childCount; i++)
        {
            EnemyInfoUI.transform.GetChild(i).gameObject.SetActive(false);
        }
        EnemyInfoUI.SetActive(false);
    }

    private void PopulateBodyInventory(GameObject inventoryUI, InventoryManager inventoryManager, GameObject owner)
    {
        for (var i = 0; i < inventoryUI.transform.GetChild(1).childCount; i++)
        {
            PopulateItem(i, inventoryManager, inventoryUI, owner);
        }
    }

    private void PopulateItem(int index, InventoryManager inventoryManager, GameObject inventoryUI, GameObject owner)
    {
        InventoryItem item;
        InventoryLocationEnum location;
        switch (index)
        {
            case 0:
                item = inventoryManager.Inventory["head"];
                location = InventoryLocationEnum.Head;
                break;
            case 1:
                item = inventoryManager.Inventory["neck"];
                location = InventoryLocationEnum.Neck;
                break;
            case 2:
                item = inventoryManager.Inventory["torso"];
                location = InventoryLocationEnum.Torso;
                break;
            case 3:
                item = inventoryManager.Inventory["rightHand"];
                location = InventoryLocationEnum.RightHand;
                break;
            case 4:
                item = inventoryManager.Inventory["leftHand"];
                location = InventoryLocationEnum.LeftHand;
                break;
            case 5:
                item = inventoryManager.Inventory["hip"];
                location = InventoryLocationEnum.Hip;
                break;
            case 6:
                item = inventoryManager.Inventory["rightRing"];
                location = InventoryLocationEnum.RightRing;
                break;
            case 7:
                item = inventoryManager.Inventory["leftRing"];
                location = InventoryLocationEnum.LeftRing;
                break;
            case 8:
                item = inventoryManager.Inventory["feet"];
                location = InventoryLocationEnum.Feet;
                break;
            case 9:
                item = inventoryManager.Inventory["alternateRightHand"];
                location = InventoryLocationEnum.AlternateRightHand;
                break;
            case 10:
                item = inventoryManager.Inventory["alternateLeftHand"];
                location = InventoryLocationEnum.AlternateLeftHand;
                break;
            case 11:
                item = inventoryManager.Inventory["hands"];
                location = InventoryLocationEnum.Hands;
                break;
            default:
                item = new InventoryItem();
                location = InventoryLocationEnum.None;
                break;
        }

        var bodyItem = inventoryUI.transform.GetChild(1).transform.GetChild(index);

        var removeButton = bodyItem.transform.GetChild(1);

        var iconObj = bodyItem.transform.GetChild(0).transform.GetChild(0);
        var image = iconObj.GetComponent<Image>();
        if (item.GameObject != null)
        {
            iconObj.GetComponent<ItemStatus>().SetInvetoryItem(location, 0, item, owner);
            image.sprite = item.GameObject.GetComponent<SpriteRenderer>().sprite;
            image.enabled = true;
            var enemyController = owner.GetComponent<EnemyController>();
            if (enemyController == null || (enemyController != null && enemyController.State.Exists(s => s == StateEnum.Dead)))
            {
                removeButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                removeButton.GetComponent<Button>().interactable = false;
            }

        }
        else
        {
            iconObj.GetComponent<ItemStatus>().SetInvetoryItem(location, 0, new InventoryItem(), owner);
            image.enabled = false;
            removeButton.GetComponent<Button>().interactable = false;
        }

        var body = inventoryUI.transform.GetChild(1).gameObject;
        if (inventoryManager.IsUsingAlternateWeapons)
        {
            body.transform.GetChild(3).gameObject.SetActive(false);
            body.transform.GetChild(4).gameObject.SetActive(false);
            body.transform.GetChild(9).gameObject.SetActive(true);
            body.transform.GetChild(10).gameObject.SetActive(true);
        }
        else
        {
            body.transform.GetChild(3).gameObject.SetActive(true);
            body.transform.GetChild(4).gameObject.SetActive(true);
            body.transform.GetChild(9).gameObject.SetActive(false);
            body.transform.GetChild(10).gameObject.SetActive(false);
        }
    }

    private void PopulateInventoryItems(GameObject inventoryUI, InventoryManager inventoryManager, GameObject owner, int maxInventorySlots)
    {
        var inventoryItems = inventoryUI.transform.GetChild(2);
        for (var i = 0; i < inventoryItems.childCount; i++)
        {
            if (i >= maxInventorySlots)
            {
                var inventoryItem = inventoryItems.transform.GetChild(i);
                var removeButton = inventoryItem.transform.GetChild(1);
                var iconObj = inventoryItem.transform.GetChild(0).transform.GetChild(0);
                iconObj.GetComponent<ItemStatus>().SetInvetoryItem(InventoryLocationEnum.None, -1, new InventoryItem(), owner);
                var image = iconObj.GetComponent<Image>();
                image.sprite = LockSprite;
                image.enabled = true;
                removeButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                if (inventoryManager.Inventory["inventory" + i].GameObject != null)
                {
                    var inventoryItem = inventoryItems.transform.GetChild(i);
                    var removeButton = inventoryItem.transform.GetChild(1);
                    var iconObj = inventoryItem.transform.GetChild(0).transform.GetChild(0);
                    iconObj.GetComponent<ItemStatus>().SetInvetoryItem(InventoryLocationEnum.Inventory, i, inventoryManager.Inventory["inventory" + i], owner);
                    var image = iconObj.GetComponent<Image>();
                    image.sprite = inventoryManager.Inventory["inventory" + i].GameObject.GetComponent<SpriteRenderer>().sprite;
                    image.enabled = true;

                    var enemyController = owner.GetComponent<EnemyController>();
                    if (enemyController == null || (enemyController != null && enemyController.State.Exists(s => s == StateEnum.Dead)))
                    {
                        removeButton.GetComponent<Button>().interactable = true;
                    }
                    else
                    {
                        removeButton.GetComponent<Button>().interactable = false;
                    }
                }
                else
                {
                    var inventoryItem = inventoryItems.transform.GetChild(i);
                    var removeButton = inventoryItem.transform.GetChild(1);
                    var iconObj = inventoryItem.transform.GetChild(0).transform.GetChild(0);
                    iconObj.GetComponent<ItemStatus>().SetInvetoryItem(InventoryLocationEnum.Inventory, i, new InventoryItem(), owner);
                    var image = iconObj.GetComponent<Image>();
                    image.enabled = false;
                    removeButton.GetComponent<Button>().interactable = false;
                }
            }
        }
    }
}
