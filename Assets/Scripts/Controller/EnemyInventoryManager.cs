using System.Collections.Generic;
using System.Linq;

public class EnemyInventoryManager : InventoryManager
{
    private EnemyController enemyController { get; set; }

    public override void Start()
    {
        //enemyController.Stats.RightHandAttack.Attack.Item = Inventory["rightHand"];
        //enemyController.Stats.LeftHandAttack.Attack.Item = Inventory["leftHand"];
    }

    public override void SetupInventory()
    {
        enemyController = GetComponent<EnemyController>();
        uiManager = FindObjectOfType<UIManager>();
        //SetupInventory();

        gameManager = FindObjectOfType<GameManager>();
        Inventory = new Dictionary<string, InventoryItem>();
        Inventory.Add("head", new InventoryItem());
        Inventory.Add("neck", new InventoryItem());
        Inventory.Add("torso", new InventoryItem());
        //Inventory.Add("rightHand", new InventoryItem(gameManager.InventoryItems["ranged1"], true));
        Inventory.Add("rightHand", new InventoryItem(gameManager.InventoryItems["bow1"], true));
        Inventory.Add("leftHand", new InventoryItem(gameManager.InventoryItems["ammo1"], true));
        Inventory.Add("hip", new InventoryItem());
        Inventory.Add("hands", new InventoryItem());
        Inventory.Add("rightRing", new InventoryItem());
        Inventory.Add("leftRing", new InventoryItem());
        Inventory.Add("feet", new InventoryItem());
        Inventory.Add("alternateRightHand", new InventoryItem());
        Inventory.Add("alternateLeftHand", new InventoryItem());

        UpdateStats(enemyController.Stats);
        for (var i = 0; i < enemyController.Stats.MaxInventorySlots; i++)
        {
            Inventory.Add("inventory" + i, new InventoryItem());
        }

        enemyController.Stats.RightHandAttack.Attack.Item = new InventoryItem(Inventory["rightHand"], false);
        enemyController.Stats.LeftHandAttack.Attack.Item = new InventoryItem(Inventory["leftHand"], false);
    }

    public override void UpdateStats(Stats stats)
    {
        base.UpdateStats(stats);
        int slots = Inventory["hip"].InventorySlots != null ? (int)Inventory["hip"].InventorySlots.Value : 0;
        stats.MaxInventorySlots = Constants.ENEMY_DEFAULT_MAX_INVENTORY_SLOTS + slots;
    }

    public override void RecalculateSpeed(Stats stats)
    {
        if (!enemyController.State.Exists(s => s == StateEnum.Running))
        {
            if (!enemyController.State.Exists(s => s == StateEnum.Crouching) && !enemyController.State.Exists(s => s == StateEnum.Dragging))
            {
                SetSpeed(Constants.PLAYER_NORMAL_SPEED, stats);
            }
            else if (enemyController.State.Exists(s => s == StateEnum.Crouching))
            {
                SetSpeed(Constants.PLAYER_CROUCH_SPEED, stats);
            }
            else if (enemyController.State.Exists(s => s == StateEnum.Dragging))
            {
                SetSpeed(Constants.PLAYER_DRAG_SPEED, stats);
            }
        }
        else if (enemyController.State.Exists(s => s == StateEnum.Running))
        {
            if (!enemyController.State.Exists(s => s == StateEnum.Crouching) && !enemyController.State.Exists(s => s == StateEnum.Dragging))
            {
                SetSpeed(Constants.PLAYER_RUN_SPEED, stats);
            }
            else if (enemyController.State.Exists(s => s == StateEnum.Crouching))
            {
                SetSpeed(Constants.PLAYER_CROUCH_RUN_SPEED, stats);
            }
            else if (enemyController.State.Exists(s => s == StateEnum.Dragging))
            {
                SetSpeed(Constants.PLAYER_DRAG_SPEED, stats);
            }
        }
    }

    public InventoryItem GetBestWeapon(Stats stats, List<Damage> playerResistances, AttackDistanceEnum attackDistance)
    {
        InventoryItem item = null;
        if (attackDistance == AttackDistanceEnum.Close)
        {
            item = GetBestMeleeWeapon();
            //item = new InventoryItem(gameManager.InventoryItems["sword1"], true);
        }

        if (attackDistance == AttackDistanceEnum.Far)
        {
            item = GetBestRangedWeapon();
            if (item == null)
            {
                item = GetBestMeleeWeapon();
            }
            //item = new InventoryItem(gameManager.InventoryItems["bow1"], true);
        }

        if (item == null)
        {
            item = GetFist(stats);
        }

        return item;
    }

    private bool IsMeleeWeapon(string hand)
    {
        return (Inventory[hand].ItemCategory == ItemCategoryEnum.Axe
            || Inventory[hand].ItemCategory == ItemCategoryEnum.Club
            || Inventory[hand].ItemCategory == ItemCategoryEnum.Dagger
            || Inventory[hand].ItemCategory == ItemCategoryEnum.Hammer
            || Inventory[hand].ItemCategory == ItemCategoryEnum.Spear
            || Inventory[hand].ItemCategory == ItemCategoryEnum.Sword
            || Inventory[hand].ItemCategory == ItemCategoryEnum.Other)
                && Inventory[hand].Type == ItemTypeEnum.Weapon
                && Inventory[hand].Range.Value <= 2
                && Inventory[hand].Durability.Value > 0;
    }

    private bool IsRangedWeapon(string hand, string otherHand)
    {
        return (Inventory[hand].ItemCategory == ItemCategoryEnum.Bow
                || Inventory[hand].ItemCategory == ItemCategoryEnum.Crossbow
                || Inventory[hand].ItemCategory == ItemCategoryEnum.Other)
                    && Inventory[hand].Type == ItemTypeEnum.Weapon
                    && Inventory[hand].Range.Value >= 2
                    && Inventory[hand].Durability.Value > 0
                    && Inventory[otherHand].Quantity.Value > 0;
    }

    private InventoryItem GetBestMeleeWeapon()
    {
        InventoryItem item = null;
        //item = new InventoryItem();
        if (IsMeleeWeapon("leftHand"))
        {
            item = new InventoryItem(Inventory["leftHand"], true);
        }
        if (IsMeleeWeapon("rightHand"))
        {
            if (item != null)
            {
                if (CompareTotalDamage(item, Inventory["rightHand"]) == -1)
                {
                    item = new InventoryItem(Inventory["rightHand"], true);
                }
            }
            else
            {
                item = new InventoryItem(Inventory["rightHand"], true);
            }
        }
        if (IsMeleeWeapon("alternateLeftHand"))
        {
            if (item != null)
            {
                if (CompareTotalDamage(item, Inventory["alternateLeftHand"]) == -1)
                {
                    item = new InventoryItem(Inventory["alternateLeftHand"], true);
                }
            }
            else
            {
                item = new InventoryItem(Inventory["alternateLeftHand"], true);
            }
        }
        if (IsMeleeWeapon("alternateRightHand"))
        {
            if (item != null)
            {
                if (CompareTotalDamage(item, Inventory["alternateRightHand"]) == -1)
                {
                    item = new InventoryItem(Inventory["alternateRightHand"], true);
                }
            }
            else
            {
                item = new InventoryItem(Inventory["alternateRightHand"], true);
            }
        }

        //item = new InventoryItem(gameManager.InventoryItems["sword1"], true);
        return item;
    }

    private InventoryItem GetBestRangedWeapon()
    {
        InventoryItem item = null;
        if (IsRangedWeapon("leftHand", "rightHand"))
        {
            item = new InventoryItem(Inventory["leftHand"], true);
        }
        if (IsRangedWeapon("rightHand", "leftHand"))
        {
            if (item != null)
            {
                if (CompareTotalDamage(item, Inventory["rightHand"]) == -1)
                {
                    item = new InventoryItem(Inventory["rightHand"], true);
                }
            }
            else
            {
                item = new InventoryItem(Inventory["rightHand"], true);
            }
        }
        if (IsRangedWeapon("alternateLeftHand", "alternateRightHand"))
        {
            if (item != null)
            {
                if (CompareTotalDamage(item, Inventory["alternateLeftHand"]) == -1)
                {
                    item = new InventoryItem(Inventory["alternateLeftHand"], true);
                }
            }
            else
            {
                item = new InventoryItem(Inventory["alternateLeftHand"], true);
            }
        }
        if (IsRangedWeapon("alternateRightHand", "alternateLeftHand"))
        {
            if (item != null)
            {
                if (CompareTotalDamage(item, Inventory["alternateRightHand"]) == -1)
                {
                    item = new InventoryItem(Inventory["alternateRightHand"], true);
                }
            }
            else
            {
                item = new InventoryItem(Inventory["alternateRightHand"], true);
            }
        }

        //item = new InventoryItem(gameManager.InventoryItems["bow1"], true);
        return item;
    }

    private int CompareTotalDamage(InventoryItem item, InventoryItem itemToCompare)
    {
        float itemSum = 0;
        float itemToCompareSum = 0;
        foreach (var dmgType in item.Damage.Keys)
        {
            itemSum += item.Damage[dmgType].MaxValue;
        }
        foreach (var dmgType in itemToCompare.Damage.Keys)
        {
            itemToCompareSum += itemToCompare.Damage[dmgType].MaxValue;
        }

        if (itemSum > itemToCompareSum)
        {
            return 1;
        }
        else if (itemSum == itemToCompareSum)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }
}