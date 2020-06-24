using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Dictionary<string, InventoryItem> Inventory { get; set; }

    public bool IsUsingAlternateWeapons = false;
    public UIManager uiManager { get; set; }

    public GameManager gameManager { get; set; }
    private PlayerController playerController { get; set; }

    public virtual void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerController = GetComponent<PlayerController>();
        uiManager = FindObjectOfType<UIManager>();
        SetupInventory();
        playerController.Stats.RightHandAttack.Attack.Item = Inventory["rightHand"];
        playerController.Stats.LeftHandAttack.Attack.Item = Inventory["leftHand"];
    }

    public void SwitchWeapons()
    {
        IsUsingAlternateWeapons = !IsUsingAlternateWeapons;
        if (IsUsingAlternateWeapons)
        {
            playerController.Stats.RightHandAttack.Attack.Replace(Inventory["alternateRightHand"], true);
            playerController.Stats.LeftHandAttack.Attack.Replace(Inventory["alternateLeftHand"], true);
        }
        else
        {
            if (playerController.Stats.RightHandAttack.Attack.OldItem != null)
            {
                playerController.Stats.RightHandAttack.Attack.Replace(playerController.Stats.RightHandAttack.Attack.OldItem);
            }
            else if (playerController.Stats.RightHandAttack.Attack.OldSkill != null)
            {
                playerController.Stats.RightHandAttack.Attack.Replace(playerController.Stats.RightHandAttack.Attack.OldSkill);
            }
            else
            {
                playerController.Stats.RightHandAttack.Attack.Replace(Inventory["rightHand"]);
            }

            if (playerController.Stats.LeftHandAttack.Attack.OldItem != null)
            {
                playerController.Stats.LeftHandAttack.Attack.Replace(playerController.Stats.LeftHandAttack.Attack.OldItem);
            }
            else if (playerController.Stats.LeftHandAttack.Attack.OldSkill != null)
            {
                playerController.Stats.LeftHandAttack.Attack.Replace(playerController.Stats.LeftHandAttack.Attack.OldSkill);
            }
            else
            {
                playerController.Stats.LeftHandAttack.Attack.Replace(Inventory["leftHand"]);
            }
        }

        UpdateStatsAndRefreshUI(playerController.Stats);
    }

    public void UpdateStatsAndRefreshUI(Stats stats, GameObject owner = null)
    {
        UpdateStats(stats);
        uiManager.RefreshInventory(owner);
    }

    public int GetInventorySlotsCount()
    {
        var count = 0;
        for (var i = 0; i < playerController.Stats.MaxInventorySlots; i++)
        {
            if (Inventory["inventory" + i].GameObject != null)
            {
                count++;
            }
        }
        return count;
    }

    public int GetEmptyInventorySlotIndex()
    {
        var index = 0;
        for (var i = 0; i < playerController.Stats.MaxInventorySlots; i++)
        {
            if (Inventory["inventory" + i].GameObject == null)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    private Stats GetOwnerStats(GameObject owner)
    {
        Stats stats = owner.GetComponent<EnemyController>() == null
            ? owner.GetComponent<PlayerController>().Stats
            : owner.GetComponent<EnemyController>().Stats;
        return stats;
    }

    public void DropItem(InventoryLocationEnum location, int index, GameObject owner)
    {
        Stats stats = GetOwnerStats(owner);
        GameObject pickups = GameObject.Find("Pickups");

        var item = GetItemByLocationAndIndex(location, index);
        SetItemByLocationAndIndex(location, index, new InventoryItem(), owner);
        var pickup = Instantiate(item.GameObject, pickups.transform);
        pickup.GetComponent<Pickable>().SetInventoryItem(new InventoryItem(item));
        pickup.transform.position = owner.transform.position;
        UpdateStatsAndRefreshUI(stats, owner);
    }

    public void RemoveItem(InventoryLocationEnum location, int index, GameObject owner)
    {
        var item = GetItemByLocationAndIndex(location, index);
        SetItemByLocationAndIndex(location, index, new InventoryItem(), owner);
        UpdateStats(GetOwnerStats(owner));
    }

    public bool PickUp(InventoryItem inventoryItem)
    {
        if (Inventory["leftHand"] != null && inventoryItem.Type == Inventory["leftHand"].Type && inventoryItem.ItemCategory == Inventory["leftHand"].ItemCategory)
        {
            Inventory["leftHand"].Quantity.Value += inventoryItem.Quantity.Value;
            inventoryItem.Quantity.Value = 0;
            if (Inventory["leftHand"].Quantity.Value > Inventory["leftHand"].Quantity.MaxValue)
            {
                inventoryItem.Quantity.Value = Inventory["leftHand"].Quantity.Value - Inventory["leftHand"].Quantity.MaxValue;
                Inventory["leftHand"].Quantity.Value = Inventory["leftHand"].Quantity.MaxValue;
            }
        }
        else if (Inventory["alternateLeftHand"] != null && inventoryItem.Type == Inventory["alternateLeftHand"].Type && inventoryItem.ItemCategory == Inventory["alternateLeftHand"].ItemCategory)
        {
            Inventory["alternateLeftHand"].Quantity.Value += inventoryItem.Quantity.Value;
            inventoryItem.Quantity.Value = 0;
            if (Inventory["alternateLeftHand"].Quantity.Value > Inventory["alternateLeftHand"].Quantity.MaxValue)
            {
                inventoryItem.Quantity.Value = Inventory["alternateLeftHand"].Quantity.Value - Inventory["alternateLeftHand"].Quantity.MaxValue;
                Inventory["alternateLeftHand"].Quantity.Value = Inventory["alternateLeftHand"].Quantity.MaxValue;
            }
        }

        //Add to inventory
        var fullSlots = GetInventorySlotsCount();
        if (fullSlots < playerController.Stats.MaxInventorySlots)
        {
            var index = GetEmptyInventorySlotIndex();
            Inventory["inventory" + index] = new InventoryItem(inventoryItem);
            uiManager.RefreshInventory();
            return true;
        }
        return false;
    }

    public void SwitchItems(InventoryLocationEnum itemLocation, int itemIndex, GameObject itemOwner, InventoryLocationEnum itemToReplaceLocation, int itemToReplaceIndex, GameObject itemToReplaceOwner)
    {
        InventoryItem iitemLocation = GetItemByLocationAndIndex(itemLocation, itemIndex, itemOwner);
        InventoryItem iitemToReplaceLocation = GetItemByLocationAndIndex(itemToReplaceLocation, itemToReplaceIndex, itemToReplaceOwner);

        if (CanSwitchItem(iitemLocation, iitemToReplaceLocation, itemLocation, itemToReplaceLocation))
        {
            SetItemByLocationAndIndex(itemLocation, itemIndex, iitemToReplaceLocation, itemOwner);
            SetItemByLocationAndIndex(itemToReplaceLocation, itemToReplaceIndex, iitemLocation, itemToReplaceOwner);
        }
        if (itemOwner != itemToReplaceOwner)
        {
            UpdateStatsAndRefreshUI(GetOwnerStats(itemToReplaceOwner), itemToReplaceOwner);
        }
        UpdateStatsAndRefreshUI(GetOwnerStats(itemOwner), itemOwner);
    }

    private bool CanSwitchItem(InventoryItem item, InventoryItem itemToReplace, InventoryLocationEnum itemLocation, InventoryLocationEnum itemToReplaceLocation)
    {
        if (item.Type == itemToReplace.Type
            || gameManager.InventorySlotType[itemToReplaceLocation].Contains(item.Type) && gameManager.InventorySlotType[itemLocation].Contains(itemToReplace.Type))
        {
            return true;
        }
        return false;
    }

    public InventoryItem GetItemByLocationAndIndex(InventoryLocationEnum itemLocation, int itemIndex, GameObject owner = null)
    {
        if (itemLocation != InventoryLocationEnum.None)
        {
            var iitemLocation = Helpers.GetItemLocationString(itemLocation, itemIndex);
            if (owner == null || owner.GetComponent<EnemyController>() == null)
            {
                return Inventory[iitemLocation];
            }
            else
            {
                return owner.GetComponent<EnemyController>().InventoryManager.Inventory[iitemLocation];
            }
        }
        return new InventoryItem();
    }

    public void SetItemByLocationAndIndex(InventoryLocationEnum itemLocation, int itemIndex, InventoryItem inventoryItem, GameObject itemOwner)
    {
        string iitemLocation = Helpers.GetItemLocationString(itemLocation, itemIndex);
        Stats itemOwnerStats = GetOwnerStats(itemOwner);

        if (itemLocation == InventoryLocationEnum.Inventory)
        {
            if (playerController != null)
            {
                bool shouldRefreshHotbar = false;
                foreach (var key in playerController.Hotbar.Keys)
                {
                    if (playerController.Hotbar[key].Item != null)
                    {
                        if (playerController.Hotbar[key].Item.Compare(Inventory[Helpers.GetItemLocationString(itemLocation, itemIndex)]))
                        {
                            playerController.Hotbar[key].Item = null;
                            shouldRefreshHotbar = true;
                        }
                    }
                }
                if (shouldRefreshHotbar)
                {
                    uiManager.ResetHotbar();
                }
            }
        }

        EnemyController ec = itemOwner.GetComponent<EnemyController>();
        InventoryItem itm;
        if (ec != null)
        {
            ec.InventoryManager.Inventory[iitemLocation] = new InventoryItem(inventoryItem);
            itm = new InventoryItem(ec.InventoryManager.Inventory[iitemLocation]);
        }
        else
        {
            Inventory[iitemLocation] = new InventoryItem(inventoryItem);
            itm = new InventoryItem(Inventory[iitemLocation]);
        }
        if (itemLocation == InventoryLocationEnum.LeftHand && itemOwnerStats.LeftHandAttack.Attack.Item != null)
        {
            itemOwnerStats.LeftHandAttack.Attack.Replace(itm, true);
        }
        if (itemLocation == InventoryLocationEnum.RightHand && itemOwnerStats.RightHandAttack.Attack.Item != null)
        {
            itemOwnerStats.RightHandAttack.Attack.Replace(itm, true);
        }

        if (itemLocation == InventoryLocationEnum.AlternateLeftHand)
        {
            itemOwnerStats.LeftHandAttack.Attack.Replace(itm);
        }
        if (itemLocation == InventoryLocationEnum.AlternateRightHand)
        {
            itemOwnerStats.RightHandAttack.Attack.Replace(itm);
        }
    }

    public void SetItemByLocationString(string location, string itemId, int tier)
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        var item = new InventoryItem(gameManager.InventoryItems[itemId], true);
        item.LevelUp(tier);
        Inventory[location] = new InventoryItem(item);
    }

    public virtual void SetupInventory()
    {
        Inventory = new Dictionary<string, InventoryItem>();

        var head = new InventoryItem(gameManager.InventoryItems["HPSBHead1"], true);
        Inventory.Add("head", head);

        var neck = new InventoryItem(gameManager.InventoryItems["amulet1"], true);
        Inventory.Add("neck", neck);

        var torso = new InventoryItem(gameManager.InventoryItems["HPSBTorso1"], true);
        Inventory.Add("torso", torso);

        //var rightHand = new InventoryItem(gameManager.InventoryItems["sword1"], true);
        var rightHand = new InventoryItem(gameManager.InventoryItems["bow1"], true);
        Inventory.Add("rightHand", rightHand);

        var leftHand = new InventoryItem(gameManager.InventoryItems["ammo1"], true);
        leftHand.Quantity.Value = 200;
        leftHand.Quantity.MaxValue = 200;
        Inventory.Add("leftHand", leftHand);

        var hip = new InventoryItem(gameManager.InventoryItems["hip1"], true);
        Inventory.Add("hip", hip);

        var hands = new InventoryItem(gameManager.InventoryItems["HPSBHands1"], true);
        Inventory.Add("hands", hands);

        var rightRing = new InventoryItem(gameManager.InventoryItems["ring1"], true);
        Inventory.Add("rightRing", rightRing);

        var leftRing = new InventoryItem(gameManager.InventoryItems["ring2"], true);
        Inventory.Add("leftRing", leftRing);

        var feet = new InventoryItem(gameManager.InventoryItems["HPSBFeet1"], true);
        Inventory.Add("feet", feet);

        //var alternateRightHand = new InventoryItem(gameManager.InventoryItems["bow1"], true);
        var alternateRightHand = new InventoryItem(gameManager.InventoryItems["sword1"], true);
        Inventory.Add("alternateRightHand", alternateRightHand);

        var alternateLeftHand = new InventoryItem(gameManager.InventoryItems["sword1"], true);
        Inventory.Add("alternateLeftHand", alternateLeftHand);

        for (var i = 0; i < Constants.DEFAULT_MAX_INVENTORY_SLOTS; i++)
        {
            Inventory.Add("inventory" + i, new InventoryItem());
        }
        playerController.Stats.MaxInventorySlots = 10;
        Inventory["inventory0"] = new InventoryItem(gameManager.InventoryItems["consumable1"], true);
        Inventory["inventory1"] = new InventoryItem(gameManager.InventoryItems["consumable2"], true);
        Inventory["inventory2"] = new InventoryItem(gameManager.InventoryItems["consumable3"], true);

        UpdateStats(playerController.Stats);
    }

    public void SetupInventory(int level)
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        int tier = ((level - 1) / 5) + 1;
        if (tier > 5)
        {
            tier = 5;
        }

        SetInventoryItemAndLevelUp(tier, "head", gameManager.ItemsByLocation[ItemTypeEnum.Head]);
        SetInventoryItemAndLevelUp(tier, "neck", gameManager.ItemsByLocation[ItemTypeEnum.Neck]);
        SetInventoryItemAndLevelUp(tier, "torso", gameManager.ItemsByLocation[ItemTypeEnum.Armor]);
        SetInventoryItemAndLevelUp(tier, "rightHand", gameManager.ItemsByLocation[ItemTypeEnum.Weapon]);
        SetWeapons(tier, "rightHand", "leftHand");
        SetInventoryItemAndLevelUp(tier, "alternateRightHand", gameManager.ItemsByLocation[ItemTypeEnum.Weapon]);
        SetWeapons(tier, "alternateRightHand", "alternateLeftHand");
        SetInventoryItemAndLevelUp(tier, "hip", gameManager.ItemsByLocation[ItemTypeEnum.Hip]);
        SetInventoryItemAndLevelUp(tier, "leftRing", gameManager.ItemsByLocation[ItemTypeEnum.Ring]);
        SetInventoryItemAndLevelUp(tier, "rightRing", gameManager.ItemsByLocation[ItemTypeEnum.Ring]);
        SetInventoryItemAndLevelUp(tier, "hands", gameManager.ItemsByLocation[ItemTypeEnum.Hands]);
        SetInventoryItemAndLevelUp(tier, "feet", gameManager.ItemsByLocation[ItemTypeEnum.Feet]);
    }

    private void SetWeapons(int tier, string rightLocation, string leftLocation)
    {
        if (Inventory[rightLocation].ItemCategory == ItemCategoryEnum.Bow)
        {
            List<InventoryItem> bowAmmo = gameManager.ItemsByLocation[ItemTypeEnum.Ammo].FindAll(i => i.ItemCategory == ItemCategoryEnum.BowAmmo);
            SetInventoryItemAndLevelUp(tier, leftLocation, bowAmmo, true);
        }
        else if (Inventory[rightLocation].ItemCategory == ItemCategoryEnum.Crossbow)
        {
            List<InventoryItem> crossbowAmmo = gameManager.ItemsByLocation[ItemTypeEnum.Ammo].FindAll(i => i.ItemCategory == ItemCategoryEnum.CrossbowAmmo);
            SetInventoryItemAndLevelUp(tier, leftLocation, crossbowAmmo, true);
        }
        else
        {
            List<InventoryItem> leftHandItems = new List<InventoryItem>();
            int rand = Random.Range(0, 2);
            if (rand == 0)
            {
                leftHandItems = gameManager.ItemsByLocation[ItemTypeEnum.Weapon];
            }
            else if (rand == 1)
            {
                leftHandItems = gameManager.ItemsByLocation[ItemTypeEnum.Shield];
            }
            SetInventoryItemAndLevelUp(tier, leftLocation, leftHandItems);
            if (Inventory[rightLocation].Type == ItemTypeEnum.None)
            {
                if (Inventory[leftLocation].ItemCategory == ItemCategoryEnum.Bow)
                {
                    List<InventoryItem> bowAmmo = gameManager.ItemsByLocation[ItemTypeEnum.Ammo].FindAll(i => i.ItemCategory == ItemCategoryEnum.BowAmmo);
                    SetInventoryItemAndLevelUp(tier, rightLocation, bowAmmo, true);
                }
                else if (Inventory[leftLocation].ItemCategory == ItemCategoryEnum.Crossbow)
                {
                    List<InventoryItem> crossbowAmmo = gameManager.ItemsByLocation[ItemTypeEnum.Ammo].FindAll(i => i.ItemCategory == ItemCategoryEnum.CrossbowAmmo);
                    SetInventoryItemAndLevelUp(tier, rightLocation, crossbowAmmo, true);
                }
            }
        }
    }

    private void SetInventoryItemAndLevelUp(int tier, string location, List<InventoryItem> items, bool shouldInclude = false)
    {
        int maxRange = shouldInclude ? items.Count : items.Count + 1;
        int rand = Random.Range(0, maxRange);
        InventoryItem item;
        if (rand < items.Count)
        {
            item = new InventoryItem(items[rand], true);
        }
        else
        {
            item = new InventoryItem();
        }
        if (tier > 1)
        {
            item.LevelUp(tier);
        }
        Inventory[location] = new InventoryItem(item);
    }

    public virtual void UpdateStats(Stats stats)
    {
        ReturnStatsToDefault(stats);

        RecalculateSpecialStats(stats);

        RecalculateSpeed(stats);
        RecalculateAttackPower(stats);
        RecalculateDefense(stats);

        stats.TotalStamina += Helpers.GetPercentOfValue(stats.MaxStamina, stats.TotalAttributes.Constitution);
        stats.TotalMaxStamina += Helpers.GetPercentOfValue(stats.MaxStamina, stats.TotalAttributes.Constitution);
        stats.TotalStaminaRecoveryTime -= Helpers.GetPercentOfValue(stats.StaminaRecoveryTime, stats.TotalAttributes.Constitution);

        var hasSkill = false;
        stats.TotalHealth += Helpers.GetPercentOfValue(stats.MaxHealth, stats.TotalAttributes.Constitution);
        stats.TotalMaxHealth += Helpers.GetPercentOfValue(stats.MaxHealth, stats.TotalAttributes.Constitution);
        if (hasSkill)
        {
            stats.TotalHealthRecoveryTime -= Helpers.GetPercentOfValue(stats.HealthRecoveryTime, stats.TotalAttributes.Constitution);
        }

        stats.TotalMana += Helpers.GetPercentOfValue(stats.MaxMana, stats.TotalAttributes.Intelligence);
        stats.TotalMaxMana += Helpers.GetPercentOfValue(stats.MaxMana, stats.TotalAttributes.Intelligence);
        if (hasSkill)
        {
            stats.TotalManaRecoveryTime -= Helpers.GetPercentOfValue(stats.ManaRecoveryTime, stats.TotalAttributes.Constitution);
        }

        stats.TotalResistances[DamageTypeEnum.Bludgeoning] += stats.TotalAttributes.Constitution;
        stats.TotalResistances[DamageTypeEnum.Piercing] += stats.TotalAttributes.Constitution;
        stats.TotalResistances[DamageTypeEnum.Slashing] += stats.TotalAttributes.Constitution;

        stats.TotalResistances[DamageTypeEnum.Cold] += stats.TotalAttributes.Intelligence;
        stats.TotalResistances[DamageTypeEnum.Fire] += stats.TotalAttributes.Intelligence;
        stats.TotalResistances[DamageTypeEnum.Lightning] += stats.TotalAttributes.Intelligence;
        stats.TotalResistances[DamageTypeEnum.Poison] += stats.TotalAttributes.Intelligence;
    }

    public void ReturnStatsToDefault(Stats stats)
    {
        stats.TotalSpeed = 0;

        stats.TotalStamina = stats.Stamina;
        stats.TotalMaxStamina = stats.MaxStamina;
        stats.TotalStaminaRecoveryTime = stats.StaminaRecoveryTime;

        stats.TotalCriticalChance = stats.CriticalChance + (Inventory["hands"].Type != ItemTypeEnum.None ? (int)Inventory["hands"].CriticalChance.Value : 0);

        stats.TotalHealth = stats.Health;
        stats.TotalMaxHealth = stats.MaxHealth;
        stats.TotalHealthRecoveryTime = stats.HealthRecoveryTime;

        stats.TotalMana = stats.Mana;
        stats.TotalMaxMana = stats.MaxMana;
        stats.TotalManaRecoveryTime = stats.ManaRecoveryTime;

        stats.TotalAttributes.Strength = stats.Attributes.Strength;
        stats.TotalAttributes.Dexterity = stats.Attributes.Dexterity;
        stats.TotalAttributes.Constitution = stats.Attributes.Constitution;
        stats.TotalAttributes.Intelligence = stats.Attributes.Intelligence;
        stats.TotalAttributes.Charisma = stats.Attributes.Charisma;

        foreach (var dmgType in stats.Resistances.Keys)
        {
            stats.TotalResistances[dmgType] = stats.Resistances[dmgType];
        }

        int hipSlots = Inventory["hip"].Type != ItemTypeEnum.None ? (int)Inventory["hip"].InventorySlots.Value : 0;
        stats.MaxInventorySlots = Constants.PLAYER_DEFAULT_MAX_INVENTORY_SLOTS + hipSlots;

        foreach (var dmgType in stats.RightHandAttack.Damage.Keys)
        {
            stats.RightHandAttack.Damage[dmgType].MinValue = 0;
            stats.RightHandAttack.Damage[dmgType].MaxValue = 0;

            stats.LeftHandAttack.Damage[dmgType].MinValue = 0;
            stats.LeftHandAttack.Damage[dmgType].MaxValue = 0;
        }
        stats.RightHandAttack.AttackRecoveryTime = 0;
        stats.RightHandAttack.StaminaConsumption = 0;
        stats.RightHandAttack.ChargeAttackSpeed = 0;

        stats.LeftHandAttack.AttackRecoveryTime = 0;
        stats.LeftHandAttack.StaminaConsumption = 0;
        stats.LeftHandAttack.ChargeAttackSpeed = 0;
    }

    public virtual void RecalculateSpeed(Stats stats)
    {
        if (!playerController.State.Exists(s => s == StateEnum.Running))
        {
            if (!playerController.State.Exists(s => s == StateEnum.Crouching) && !playerController.State.Exists(s => s == StateEnum.Dragging))
            {
                SetSpeed(Constants.PLAYER_NORMAL_SPEED, stats);
            }
            else if (playerController.State.Exists(s => s == StateEnum.Crouching))
            {
                SetSpeed(Constants.PLAYER_CROUCH_SPEED, stats);
            }
            else if (playerController.State.Exists(s => s == StateEnum.Dragging))
            {
                SetSpeed(Constants.PLAYER_DRAG_SPEED, stats);
            }
        }
        else if (playerController.State.Exists(s => s == StateEnum.Running))
        {
            if (!playerController.State.Exists(s => s == StateEnum.Crouching) && !playerController.State.Exists(s => s == StateEnum.Dragging))
            {
                SetSpeed(Constants.PLAYER_RUN_SPEED, stats);
            }
            else if (playerController.State.Exists(s => s == StateEnum.Crouching))
            {
                SetSpeed(Constants.PLAYER_CROUCH_RUN_SPEED, stats);
            }
            else if (playerController.State.Exists(s => s == StateEnum.Dragging))
            {
                SetSpeed(Constants.PLAYER_DRAG_SPEED, stats);
            }
        }
    }

    public void SetSpeed(float speed, Stats stats)
    {
        // TODO: Add all other body parts to this
        stats.Speed = speed + Helpers.GetPercentOfValue(speed, stats.TotalAttributes.Dexterity);
        float feetSpeedPercent = Inventory["feet"].Speed != null ? Helpers.GetPercentOfValue(stats.Speed, Inventory["feet"].Speed.Value) : 0;

        float dadvSpeed = 0;
        if (Inventory["feet"].Weight == WeightEnum.Medium)
        {
            dadvSpeed += Constants.WEIGHT_MEDIUM_DISADVANTAGE;
        }
        else if (Inventory["feet"].Weight == WeightEnum.Large)
        {
            dadvSpeed += Constants.WEIGHT_HEAVY_DISADVANTAGE;
        }
        if (Inventory["torso"].Weight == WeightEnum.Medium)
        {
            dadvSpeed += Constants.WEIGHT_MEDIUM_DISADVANTAGE;
        }
        else if (Inventory["torso"].Weight == WeightEnum.Large)
        {
            dadvSpeed += Constants.WEIGHT_HEAVY_DISADVANTAGE;
        }

        stats.TotalSpeed += stats.Speed + feetSpeedPercent + Helpers.GetPercentOfValue(stats.Speed, dadvSpeed);
    }

    public void RecalculateAttackPower(Stats stats)
    {
        string rightLocation = IsUsingAlternateWeapons ? "alternateRightHand" : "rightHand";
        string leftLocation = IsUsingAlternateWeapons ? "alternateLeftHand" : "leftHand";

        UpdateHandAttackPower(rightLocation, stats, stats.RightHandAttack);
        UpdateHandAttackPower(leftLocation, stats, stats.LeftHandAttack);
    }

    private void UpdateHandAttackPower(string location, Stats stats, AttackPower hand)
    {
        if (Inventory[location].Type == ItemTypeEnum.Weapon)
        {
            float attributeDmg = 0;
            if (Inventory[location].ItemCategory == ItemCategoryEnum.Axe
                || Inventory[location].ItemCategory == ItemCategoryEnum.Club
                || Inventory[location].ItemCategory == ItemCategoryEnum.Dagger
                || Inventory[location].ItemCategory == ItemCategoryEnum.Hammer
                || Inventory[location].ItemCategory == ItemCategoryEnum.Other
                || Inventory[location].ItemCategory == ItemCategoryEnum.Spear
                || Inventory[location].ItemCategory == ItemCategoryEnum.Sword)
            {
                // If ever modifying this, also modify in UIManager in GetDamageText for skills
                attributeDmg = Helpers.RoundToTwoDecimals((float)stats.TotalAttributes.Strength / 2);
                if (Inventory[location].Properties != null && Inventory[location].Properties.Contains(PropertiesEnum.Finesse) &&
                    stats.TotalAttributes.Dexterity > stats.TotalAttributes.Strength)
                {
                    attributeDmg = Helpers.RoundToTwoDecimals((float)stats.TotalAttributes.Dexterity / 2);
                }

            }
            else if (Inventory[location].ItemCategory == ItemCategoryEnum.Bow
                || Inventory[location].ItemCategory == ItemCategoryEnum.Crossbow)
            {
                attributeDmg = Helpers.RoundToTwoDecimals((float)stats.TotalAttributes.Dexterity / 2);
                if (Inventory[location].Properties != null && Inventory[location].Properties.Contains(PropertiesEnum.Finesse) &&
                    stats.TotalAttributes.Strength > stats.TotalAttributes.Dexterity)
                {
                    attributeDmg = Helpers.RoundToTwoDecimals((float)stats.TotalAttributes.Strength / 2);
                }
            }

            foreach (var dmgType in Inventory[location].Damage.Keys)
            {
                float a = attributeDmg;
                if (Inventory[location].Damage[dmgType].MinValue == 0
                    || Inventory[location].Damage[dmgType].MaxValue == 0)
                {
                    a = 0;
                }
                hand.Damage[dmgType].MinValue += Inventory[location].Damage[dmgType].MinValue + a;
                hand.Damage[dmgType].MaxValue += Inventory[location].Damage[dmgType].MaxValue + a;
            }

            float dadvSpeed = 0;
            hand.AttackRecoveryTime += Inventory[location].AttackRecoveryTime.Value - Helpers.GetPercentOfValue(Inventory[location].AttackRecoveryTime.Value, stats.TotalAttributes.Dexterity / 2);
            if (Inventory["hands"].Weight == WeightEnum.Medium)
            {
                dadvSpeed += Constants.WEIGHT_MEDIUM_DISADVANTAGE;
            }
            else if (Inventory["hands"].Weight == WeightEnum.Large)
            {
                dadvSpeed += Constants.WEIGHT_HEAVY_DISADVANTAGE;
            }
            if (Inventory["torso"].Weight == WeightEnum.Medium)
            {
                dadvSpeed += Constants.WEIGHT_MEDIUM_DISADVANTAGE;
            }
            else if (Inventory["torso"].Weight == WeightEnum.Large)
            {
                dadvSpeed += Constants.WEIGHT_HEAVY_DISADVANTAGE;
            }
            hand.AttackRecoveryTime -= Helpers.GetPercentOfValue(hand.AttackRecoveryTime, dadvSpeed);
            if (hand.AttackRecoveryTime <= 0)
            {
                hand.AttackRecoveryTime = 0;
            }

            hand.StaminaConsumption += Inventory[location].StaminaConsumption.Value - Helpers.GetPercentOfValue(Inventory[location].StaminaConsumption.Value, stats.TotalAttributes.Constitution);

            hand.ChargeAttackSpeed += Inventory[location].ChargeSpeed.Value;
            dadvSpeed = 0;
            if (Inventory["head"].Weight == WeightEnum.Medium)
            {
                dadvSpeed += Constants.WEIGHT_MEDIUM_DISADVANTAGE;
            }
            else if (Inventory["head"].Weight == WeightEnum.Large)
            {
                dadvSpeed += Constants.WEIGHT_HEAVY_DISADVANTAGE;
            }
            if (Inventory["torso"].Weight == WeightEnum.Medium)
            {
                dadvSpeed += Constants.WEIGHT_MEDIUM_DISADVANTAGE;
            }
            else if (Inventory["torso"].Weight == WeightEnum.Large)
            {
                dadvSpeed += Constants.WEIGHT_HEAVY_DISADVANTAGE;
            }
            hand.ChargeAttackSpeed -= Helpers.GetPercentOfValue(hand.ChargeAttackSpeed, dadvSpeed);
        }
        else if (Inventory[location].Type == ItemTypeEnum.None)
        {
            InventoryItem fist = GetFist(stats);
            // TODO: Add special stats from other items
            hand.AttackRecoveryTime += fist.AttackRecoveryTime.Value;
            hand.StaminaConsumption += fist.StaminaConsumption.Value;
            hand.Damage = fist.Damage;
        }
    }

    public virtual void RecalculateDefense(Stats stats)
    {
        RecalculateArmorPerPiece("torso", stats);
        RecalculateArmorPerPiece("head", stats);
        RecalculateArmorPerPiece("hands", stats);
        RecalculateArmorPerPiece("feet", stats);
        if (IsUsingAlternateWeapons)
        {
            if (Inventory["alternateLeftHand"].ItemCategory == ItemCategoryEnum.Shield)
            {
                RecalculateArmorPerPiece("alternateLeftHand", stats);
            }
            if (Inventory["alternateRightHand"].ItemCategory == ItemCategoryEnum.Shield)
            {
                RecalculateArmorPerPiece("alternateRightHand", stats);
            }
        }
        else
        {
            if (Inventory["leftHand"].ItemCategory == ItemCategoryEnum.Shield)
            {
                RecalculateArmorPerPiece("leftHand", stats);
            }
            if (Inventory["rightHand"].ItemCategory == ItemCategoryEnum.Shield)
            {
                RecalculateArmorPerPiece("rightHand", stats);
            }
        }
    }

    public float GetCurrentArmorResistanceByType(DamageTypeEnum res)
    {
        float value = 0;
        if (Inventory["torso"].Type != ItemTypeEnum.None)
        {
            value += Inventory["torso"].Resistance[res].Value;
        }
        if (Inventory["head"].Type != ItemTypeEnum.None)
        {
            value += Inventory["head"].Resistance[res].Value;
        }
        if (Inventory["hands"].Type != ItemTypeEnum.None)
        {
            value += Inventory["hands"].Resistance[res].Value;
        }
        if (Inventory["feet"].Type != ItemTypeEnum.None)
        {
            value += Inventory["feet"].Resistance[res].Value;
        }
        return value;
    }

    public void RecalculateArmorPerPiece(string location, Stats stats)
    {
        float armorPoint = 0;
        if (Inventory[location].Type != ItemTypeEnum.None)
        {
            foreach (var dmgType in Inventory[location].Resistance.Keys)
            {
                armorPoint = Inventory[location].Durability.MaxValue / Inventory[location].Resistance[dmgType].MaxValue;

                for (var i = 0; i <= Inventory[location].Resistance[dmgType].MaxValue; i++)
                {
                    var durabilityPoints = i * armorPoint;
                    if (Inventory[location].Durability.Value <= durabilityPoints)
                    {
                        // TODO: Check this logic
                        Inventory[location].Resistance[dmgType].Value = i;
                        break;
                    }
                }

                stats.TotalResistances[dmgType] += Inventory[location].Resistance[dmgType].Value;
            }
        }
    }

    public void RecalculateSpecialStats(Stats stats)
    {
        ApplySpecialStats(stats, "head");
        ApplySpecialStats(stats, "neck");
        ApplySpecialStats(stats, "torso");
        if (!IsUsingAlternateWeapons)
        {
            ApplySpecialStats(stats, "rightHand");
            ApplySpecialStats(stats, "leftHand");
        }
        else
        {
            ApplySpecialStats(stats, "alternateRightHand");
            ApplySpecialStats(stats, "alternateLeftHand");
        }
        ApplySpecialStats(stats, "hip");
        ApplySpecialStats(stats, "rightRing");
        ApplySpecialStats(stats, "leftRing");
        ApplySpecialStats(stats, "feet");
        ApplySpecialStats(stats, "hands");
    }

    public virtual void ApplySpecialStats(Stats stats, string location)
    {
        if (Inventory[location].SpecialStats != null)
        {
            if (Inventory[location].SpecialStats.Attributes != null)
            {
                stats.TotalAttributes.Strength += Inventory[location].SpecialStats.Attributes.Strength;
                stats.TotalAttributes.Dexterity += Inventory[location].SpecialStats.Attributes.Dexterity;
                stats.TotalAttributes.Constitution += Inventory[location].SpecialStats.Attributes.Constitution;
                stats.TotalAttributes.Intelligence += Inventory[location].SpecialStats.Attributes.Intelligence;
                stats.TotalAttributes.Charisma += Inventory[location].SpecialStats.Attributes.Charisma;
            }

            stats.TotalHealth += Inventory[location].SpecialStats.Health;
            stats.TotalMaxHealth += Inventory[location].SpecialStats.Health;

            stats.TotalMana += Inventory[location].SpecialStats.Mana;
            stats.TotalMaxMana += Inventory[location].SpecialStats.Mana;

            if (location == "leftHand" && !IsUsingAlternateWeapons)
            {
                UpdateLeftHandSpecialStatsDamage(location, stats);
            }
            else if (location == "alternateLeftHand" && IsUsingAlternateWeapons)
            {
                UpdateLeftHandSpecialStatsDamage(location, stats);
            }
            else if (location == "rightHand" && !IsUsingAlternateWeapons)
            {
                UpdateRightHandSpecialStatsDamage(location, stats);
            }
            else if (location == "alternateRightHand" && IsUsingAlternateWeapons)
            {
                UpdateRightHandSpecialStatsDamage(location, stats);
            }
            else
            {
                UpdateRightHandSpecialStatsDamage(location, stats);
                UpdateLeftHandSpecialStatsDamage(location, stats);
            }
            stats.TotalSpeed += Helpers.GetPercentOfValue(stats.TotalSpeed, Inventory[location].SpecialStats.Speed);
            stats.TotalStamina += Inventory[location].SpecialStats.Stamina;
            stats.TotalMaxStamina += Inventory[location].SpecialStats.Stamina;

            if (Inventory[location].SpecialStats.Resistances != null)
            {
                foreach (var resistance in Inventory[location].SpecialStats.Resistances.Keys)
                {
                    stats.TotalResistances[resistance] += Inventory[location].SpecialStats.Resistances[resistance];
                }
            }
        }
    }

    private void UpdateLeftHandSpecialStatsDamage(string location, Stats stats)
    {
        if (Inventory[location].SpecialStats.Damage != null)
        {
            string hand = IsUsingAlternateWeapons ? "alternateLeftHand" : "leftHand";

            foreach (var dmgType in Inventory[location].SpecialStats.Damage.Keys)
            {
                stats.LeftHandAttack.Damage[dmgType].MinValue += Inventory[location].SpecialStats.Damage[dmgType].MinValue;
                stats.LeftHandAttack.Damage[dmgType].MaxValue += Inventory[location].SpecialStats.Damage[dmgType].MaxValue;
            }
            stats.LeftHandAttack.AttackRecoveryTime += -1 * Helpers.GetPercentOfValue(Inventory[hand].AttackRecoveryTime.Value, Inventory[location].SpecialStats.AttackSpeedPercent);
            stats.LeftHandAttack.StaminaConsumption += -1 * Helpers.GetPercentOfValue(Inventory[hand].StaminaConsumption.Value, Inventory[location].SpecialStats.StaminaConsumptionPercent);
        }
    }

    private void UpdateRightHandSpecialStatsDamage(string location, Stats stats)
    {
        if (Inventory[location].SpecialStats.Damage != null)
        {
            string hand = IsUsingAlternateWeapons ? "alternateRightHand" : "rightHand";

            foreach (var dmgType in Inventory[location].SpecialStats.Damage.Keys)
            {
                stats.RightHandAttack.Damage[dmgType].MinValue += Inventory[location].SpecialStats.Damage[dmgType].MinValue;
                stats.RightHandAttack.Damage[dmgType].MaxValue += Inventory[location].SpecialStats.Damage[dmgType].MaxValue;
            }
            stats.RightHandAttack.AttackRecoveryTime += -1 * Helpers.GetPercentOfValue(Inventory[hand].AttackRecoveryTime.Value, Inventory[location].SpecialStats.AttackSpeedPercent);
            stats.RightHandAttack.StaminaConsumption += -1 * Helpers.GetPercentOfValue(Inventory[hand].StaminaConsumption.Value, Inventory[location].SpecialStats.StaminaConsumptionPercent);
        }
    }

    public InventoryItem GetFist(Stats stats)
    {
        InventoryItem fist = new InventoryItem();
        fist.AttackRecoveryTime.Value += .2f - Helpers.GetPercentOfValue(.2f, stats.TotalAttributes.Dexterity / 2);
        fist.StaminaConsumption.Value += .5f - Helpers.GetPercentOfValue(.5f, stats.TotalAttributes.Constitution);
        fist.Damage[DamageTypeEnum.Bludgeoning].MinValue = 1 + Helpers.RoundToTwoDecimals((float)stats.TotalAttributes.Strength / 2);
        fist.Damage[DamageTypeEnum.Bludgeoning].MaxValue = 4 + Helpers.RoundToTwoDecimals((float)stats.TotalAttributes.Strength / 2);

        return fist;
    }
}