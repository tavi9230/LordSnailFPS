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
        for (var i = 0; i < enemyController.Stats.MaxInventorySlots; i++)
        {
            Inventory.Add("inventory" + i, new InventoryItem());
        }

        UpdateStats(enemyController.Stats);

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

    public InventoryItem GetBestWeapon(List<Damage> playerResistances, AttackDistanceEnum attackDistance)
    {
        InventoryItem item = null;
        if (attackDistance == AttackDistanceEnum.Close)
        {
            //item = new InventoryItem();
            item = new InventoryItem(gameManager.InventoryItems["sword1"], true);
        }

        if (attackDistance == AttackDistanceEnum.Far)
        {
            // TODO: Check for ammo
            //item = new InventoryItem();
            item = new InventoryItem(gameManager.InventoryItems["bow1"], true);
        }

        return item;
    }
}