using System.Collections.Generic;

public class Stats
{
    public float Speed = 5f;
    public float TotalSpeed = 5f;
    public float Armor = 0;
    public Attributes Attributes;
    public Attributes TotalAttributes;
    public float Stamina = 10f;
    public float TotalStamina = 10f;
    public float MaxStamina = 10f;
    public float TotalMaxStamina = 10f;
    public float StaminaRecoveryTime = 1f;
    public float TotalStaminaRecoveryTime = 1f;
    public Dictionary<DamageTypeEnum, float> Resistances;
    public Dictionary<DamageTypeEnum, float> TotalResistances;
    public float Health;
    public float TotalHealth;
    public float MaxHealth;
    public float TotalMaxHealth;
    public float HealthRecoveryTime = 0f;
    public float TotalHealthRecoveryTime = 0f;
    public int MaxInventorySlots;
    public AttackPower RightHandAttack;
    public AttackPower LeftHandAttack;
    public float FoV;
    public float ViewDistance;
    public float Mana;
    public float MaxMana;
    public float TotalMana;
    public float TotalMaxMana;
    public float ManaRecoveryTime = 0f;
    public float TotalManaRecoveryTime = 0f;
    public int CriticalChance = 0;
    public int TotalCriticalChance = 0;
    public float ExperiencePoints = 0;
    public float StoredExperiencePoints = 0;
    public int Level = 1;
    public int AvailableAttributePoints = 0;
    public Dictionary<ActivityEnum, List<Skill>> Skills;
    public Skill ActiveSkill;
    public string Name = "Dimitri";
    public string Id = "player";
    public PreferedFightStyleEnum PreferedFightStyle;

    public Stats()
    {
        TotalAttributes = new Attributes();
        TotalResistances = new Dictionary<DamageTypeEnum, float>();
        RightHandAttack = new AttackPower();
        LeftHandAttack = new AttackPower();
        Skills = new Dictionary<ActivityEnum, List<Skill>>
        {
            { ActivityEnum.Active, new List<Skill>() },
            { ActivityEnum.Inactive, new List<Skill>() },
        };
    }

    public Stats(float speed, float armor, Attributes attributes, float stamina, float maxStamina, float staminaRecoveryTime, Dictionary<DamageTypeEnum, float> resistances,
        float currentHealth, float maxHealth, int maxInventorySlots, float fov, float viewDistance, float healthRecoveryTime, float mana, float maxMana, float manaRecoveryTime)
    {
        Speed = speed;
        Armor = armor;
        Attributes = attributes;
        Stamina = stamina;
        MaxStamina = maxStamina;
        StaminaRecoveryTime = staminaRecoveryTime;
        Resistances = resistances;
        Health = currentHealth;
        MaxHealth = maxHealth;
        MaxInventorySlots = maxInventorySlots;
        RightHandAttack = new AttackPower();
        LeftHandAttack = new AttackPower();
        FoV = fov;
        ViewDistance = viewDistance;
        HealthRecoveryTime = healthRecoveryTime;
        Mana = mana;
        MaxMana = maxMana;
        ManaRecoveryTime = manaRecoveryTime;
        Skills = new Dictionary<ActivityEnum, List<Skill>>
        {
            {ActivityEnum.Active, new List<Skill>() },
            {ActivityEnum.Inactive, new List<Skill>() }
        };

        TotalSpeed = 0;
        TotalStamina = 0;
        TotalMaxStamina = 0;
        TotalHealthRecoveryTime = 0;
        TotalMaxHealth = 0;
        TotalHealth = 0;
        TotalStaminaRecoveryTime = 0;
        TotalMana = 0;
        TotalMaxMana = 0;
        TotalManaRecoveryTime = 0;

        CriticalChance = 0;
        TotalCriticalChance = 0;

        TotalResistances = new Dictionary<DamageTypeEnum, float>();
        foreach (var resistType in Resistances.Keys)
        {
            TotalResistances.Add(resistType, 0);
        }
        TotalAttributes = new Attributes();
    }

    public Skill GetBestSkill(List<Damage> playerResistances, AttackDistanceEnum attackDistance)
    {
        Skill skill = null;
        if (Skills[ActivityEnum.Active].Count > 0)
        {
            int rand = UnityEngine.Random.Range(0, Skills[ActivityEnum.Active].Count);

            if (attackDistance == AttackDistanceEnum.Close)
            {
                skill = new Skill(Skills[ActivityEnum.Active][rand]);
            }

            else if (attackDistance == AttackDistanceEnum.Far)
            {
                skill = new Skill(Skills[ActivityEnum.Active][rand]);
            }
        }

        return skill;
    }
}