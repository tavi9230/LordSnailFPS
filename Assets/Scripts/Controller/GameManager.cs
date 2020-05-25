using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> CombatEnemyList;
    public List<GameObject> EnemyList;
    public Dictionary<string, InventoryItem> InventoryItems;
    public Dictionary<ItemTypeEnum, List<InventoryItem>> ItemsByLocation;
    public Dictionary<string, Skill> Skills;
    public Dictionary<string, Enemy> Enemies;
    public Dictionary<InventoryLocationEnum, List<ItemTypeEnum>> InventorySlotType;
    public Dictionary<int, int> LevelRequirements;
    public List<Transform> PatrolPoints;

    public MapManager MapManager;
    public bool isDebugging = false;

    void Awake()
    {
        PatrolPoints = new List<Transform>();
        var patrolPoints = GameObject.FindGameObjectWithTag("PatrolPoint");
        for (var i = 0; i < patrolPoints.transform.childCount; i++)
        {
            PatrolPoints.Add(patrolPoints.transform.GetChild(i));
        }
        CombatEnemyList = new List<GameObject>();
        EnemyList = new List<GameObject>();
        InventoryItems = ObjectLoader.LoadItems();
        ItemsByLocation = ObjectLoader.GetItemsByLocation();
        Skills = SkillLoader.LoadSkills();
        Enemies = EnemyLoader.LoadEnemies();
        SetInventorySlotType();
        SetLevelRequirements();

        SpawnEnemy(Enemies["human_2"], new Vector3(5, 1, 5), new List<Vector3> {
                new Vector3(1, 1, 1)
            }, 1);
    }

    private void SpawnEnemy(Enemy enemy, Vector3 location, List<Vector3> patrolPoints, int level = 1)
    {
        GameObject enemyObject = Instantiate(enemy.GameObject, location, Quaternion.identity);
        EnemyController enemyController = enemyObject.GetComponent<EnemyController>();

        Stats stats = new Stats();
        stats.Id = enemy.Id;
        stats.Attributes = enemy.Attributes;
        stats.AvailableAttributePoints = enemy.AvailableAttributePoints;
        stats.CriticalChance = enemy.CriticalChance;
        stats.Health = enemy.Health.GetMinMaxValues(enemy.Health).Value;
        stats.MaxHealth = stats.Health;
        stats.ExperiencePoints = enemy.ExperiencePoints;
        stats.FoV = enemy.FoV.GetMinMaxValues(enemy.FoV, true).Value;
        stats.HealthRecoveryTime = enemy.HealthRecoveryTime;
        stats.Level = level != 1 ? level : enemy.Level;
        stats.Mana = enemy.Mana;
        stats.MaxMana = stats.Mana;
        stats.ManaRecoveryTime = enemy.ManaRecoveryTime;
        var invSlots = enemy.InventorySlots.GetMinMaxValues(enemy.InventorySlots, true).Value;
        stats.MaxInventorySlots = int.Parse(invSlots.ToString());
        stats.Name = enemy.Name;
        stats.Resistances = enemy.Resistances;
        stats.Speed = enemy.Speed.GetMinMaxValues(enemy.Speed).Value;
        stats.Stamina = enemy.Stamina.GetMinMaxValues(enemy.Stamina).Value;
        stats.MaxStamina = stats.Stamina;
        stats.StaminaRecoveryTime = enemy.StaminaRecoveryTime;
        stats.StoredExperiencePoints = enemy.StoredExperience;
        stats.ViewDistance = enemy.ViewDistance.GetMinMaxValues(enemy.ViewDistance).Value;
        stats.PreferedFightStyle = enemy.PreferedFightStyle;

        if (enemy.PreferedFightStyle == PreferedFightStyleEnum.None)
        {
            int rand = Random.Range(0, 3);
            stats.PreferedFightStyle = (PreferedFightStyleEnum)rand;
        }

        enemyController.SetupEnemyStats(stats, enemy.Skills, enemy.Inventory, patrolPoints);
        EnemyList.Add(enemyObject);
    }

    private void SetInventorySlotType()
    {
        InventorySlotType = new Dictionary<InventoryLocationEnum, List<ItemTypeEnum>>();
        InventorySlotType.Add(InventoryLocationEnum.AlternateLeftHand, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Weapon,
            ItemTypeEnum.Shield,
            ItemTypeEnum.Ammo,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.AlternateRightHand, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Weapon,
            ItemTypeEnum.Shield,
            ItemTypeEnum.Ammo,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.Feet, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Feet,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.Head, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Head,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.Hip, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Hip,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.Inventory, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Weapon,
            ItemTypeEnum.Armor,
            ItemTypeEnum.Head,
            ItemTypeEnum.Neck,
            ItemTypeEnum.Ring,
            ItemTypeEnum.Shield,
            ItemTypeEnum.Feet,
            ItemTypeEnum.Hip,
            ItemTypeEnum.Consumable,
            ItemTypeEnum.Ammo,
            ItemTypeEnum.Hands,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.LeftHand, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Weapon,
            ItemTypeEnum.Shield,
            ItemTypeEnum.Ammo,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.LeftRing, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Ring,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.Neck, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Neck,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.RightHand, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Weapon,
            ItemTypeEnum.Shield,
            ItemTypeEnum.Ammo,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.RightRing, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Ring,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.Torso, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Armor,
            ItemTypeEnum.None,
        });
        InventorySlotType.Add(InventoryLocationEnum.Hands, new List<ItemTypeEnum>()
        {
            ItemTypeEnum.Hands,
            ItemTypeEnum.None,
        });
    }

    private void SetLevelRequirements()
    {
        LevelRequirements = new Dictionary<int, int>();
        for (var i = 1; i <= 50; i++)
        {
            LevelRequirements.Add(i, i * 100);
        }
    }
}
