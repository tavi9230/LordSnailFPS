using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public string Id { get; set; }
    public int Tier { get; set; }
    public ItemTypeEnum Type { get; set; }
    public ItemCategoryEnum ItemCategory { get; set; }
    public int Enchantments { get; set; }
    public int Slots { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public WeightEnum Weight { get; set; }
    public MinMax Range { get; set; }
    public MinMax Quantity { get; set; }
    public MinMax Durability { get; set; }
    public MinMax AttackRecoveryTime { get; set; }
    public MinMax Speed { get; set; }
    public MinMax StaminaConsumption { get; set; }
    public MinMax InventorySlots { get; set; }
    public MinMax CriticalChance { get; set; }
    public MinMax Regain { get; set; }
    public MinMax ChargeSpeed { get; set; }

    public Dictionary<DamageTypeEnum, Damage> Damage { get; set; }
    public Dictionary<DamageTypeEnum, Damage> Resistance { get; set; }
    public SpecialStats SpecialStats { get; set; }
    public List<PropertiesEnum> Properties { get; set; }

    public GameObject GameObject { get; set; }
    public GameObject Projectile { get; set; }

    public InventoryItem()
    {
        Type = ItemTypeEnum.None;
        Damage = new Dictionary<DamageTypeEnum, Damage>()
        {
            {DamageTypeEnum.Piercing, new Damage() { Type=DamageTypeEnum.Piercing } },
            {DamageTypeEnum.Slashing, new Damage() { Type=DamageTypeEnum.Slashing }},
            {DamageTypeEnum.Bludgeoning, new Damage() { Type=DamageTypeEnum.Bludgeoning, MinValue = 1, MaxValue = 4 }},
            {DamageTypeEnum.Fire, new Damage() { Type=DamageTypeEnum.Fire }},
            {DamageTypeEnum.Cold, new Damage() { Type=DamageTypeEnum.Cold }},
            {DamageTypeEnum.Lightning, new Damage() { Type=DamageTypeEnum.Lightning }},
            {DamageTypeEnum.Poison, new Damage() { Type=DamageTypeEnum.Poison }},
        };
        Resistance = new Dictionary<DamageTypeEnum, Damage>()
        {
            {DamageTypeEnum.Piercing, new Damage() { Type=DamageTypeEnum.Piercing } },
            {DamageTypeEnum.Slashing, new Damage() { Type=DamageTypeEnum.Slashing }},
            {DamageTypeEnum.Bludgeoning, new Damage() { Type=DamageTypeEnum.Bludgeoning }},
            {DamageTypeEnum.Fire, new Damage() { Type=DamageTypeEnum.Fire }},
            {DamageTypeEnum.Cold, new Damage() { Type=DamageTypeEnum.Cold }},
            {DamageTypeEnum.Lightning, new Damage() { Type=DamageTypeEnum.Lightning }},
            {DamageTypeEnum.Poison, new Damage() { Type=DamageTypeEnum.Poison }},
        };
        Range = new MinMax();
        Quantity = new MinMax();
        Durability = new MinMax();
        AttackRecoveryTime = new MinMax();
        Speed = new MinMax();
        StaminaConsumption = new MinMax();
        InventorySlots = new MinMax();
        CriticalChance = new MinMax();
        Regain = new MinMax();
        ChargeSpeed = new MinMax();
    }

    public bool Compare(InventoryItem item)
    {
        // TODO Check if more attributes are needed for a correct comparison
        var isEqual = true;
        if (item.Id != Id
            || item.AttackRecoveryTime != null && AttackRecoveryTime != null && item.AttackRecoveryTime.Value != AttackRecoveryTime.Value
            || item.CriticalChance != null && CriticalChance != null && item.CriticalChance.Value != item.CriticalChance.Value
            || !IsSameDamage(item)
            || item.Description != Description
            || item.Durability != null && Durability != null && item.Durability.Value != Durability.Value
            || item.Enchantments != Enchantments
            || item.InventorySlots != InventorySlots
            || item.ItemCategory != ItemCategory
            || item.Name != Name
            || item.Quantity.Value != Quantity.Value
            || item.ChargeSpeed != null && ChargeSpeed != null && item.ChargeSpeed.Value != ChargeSpeed.Value)
        {
            isEqual = false;
        }
        return isEqual;
    }

    private bool IsSameDamage(InventoryItem item)
    {
        return item.Damage[DamageTypeEnum.Bludgeoning].MinValue == Damage[DamageTypeEnum.Bludgeoning].MinValue
            && item.Damage[DamageTypeEnum.Cold].MinValue == Damage[DamageTypeEnum.Cold].MinValue
            && item.Damage[DamageTypeEnum.Fire].MinValue == Damage[DamageTypeEnum.Fire].MinValue
            && item.Damage[DamageTypeEnum.Lightning].MinValue == Damage[DamageTypeEnum.Lightning].MinValue
            && item.Damage[DamageTypeEnum.Piercing].MinValue == Damage[DamageTypeEnum.Piercing].MinValue
            && item.Damage[DamageTypeEnum.Poison].MinValue == Damage[DamageTypeEnum.Poison].MinValue
            && item.Damage[DamageTypeEnum.Slashing].MinValue == Damage[DamageTypeEnum.Slashing].MinValue

            && item.Damage[DamageTypeEnum.Bludgeoning].MaxValue == Damage[DamageTypeEnum.Bludgeoning].MaxValue
            && item.Damage[DamageTypeEnum.Cold].MaxValue == Damage[DamageTypeEnum.Cold].MaxValue
            && item.Damage[DamageTypeEnum.Fire].MaxValue == Damage[DamageTypeEnum.Fire].MaxValue
            && item.Damage[DamageTypeEnum.Lightning].MaxValue == Damage[DamageTypeEnum.Lightning].MaxValue
            && item.Damage[DamageTypeEnum.Piercing].MaxValue == Damage[DamageTypeEnum.Piercing].MaxValue
            && item.Damage[DamageTypeEnum.Poison].MaxValue == Damage[DamageTypeEnum.Poison].MaxValue
            && item.Damage[DamageTypeEnum.Slashing].MaxValue == Damage[DamageTypeEnum.Slashing].MaxValue;
    }

    public InventoryItem(InventoryItem item, bool createNew = false)
    {
        Id = item.Id;
        Resistance = new Dictionary<DamageTypeEnum, Damage>();
        if (item.Resistance != null)
        {
            foreach (var key in item.Resistance.Keys)
            {
                var dmg = new Damage()
                {
                    Type = item.Resistance[key].Type,
                    DefaultMinValue = item.Resistance[key].DefaultMinValue,
                    DefaultMaxValue = item.Resistance[key].DefaultMaxValue
                };
                if (createNew)
                {
                    dmg.Value = Random.Range((int)dmg.DefaultMinValue, (int)dmg.DefaultMaxValue + 1);

                }
                else
                {
                    dmg.Value = item.Resistance[key].Value;
                }
                dmg.MaxValue = dmg.Value;
                Resistance.Add(key, dmg);
            }
        }

        Damage = new Dictionary<DamageTypeEnum, Damage>();
        if (item.Damage != null)
        {
            foreach (var key in item.Damage.Keys)
            {
                var dmg = new Damage()
                {
                    Type = item.Damage[key].Type,
                    DefaultMinValue = item.Damage[key].DefaultMinValue,
                    DefaultMaxValue = item.Damage[key].DefaultMaxValue
                };
                if (createNew)
                {
                    dmg.MinValue = (float)System.Math.Round(Random.Range(dmg.DefaultMinValue, dmg.DefaultMaxValue) * 100f) / 100f;
                    dmg.MaxValue = (float)System.Math.Round(Random.Range(dmg.DefaultMinValue, dmg.DefaultMaxValue) * 100f) / 100f;
                    if (dmg.MinValue > dmg.MaxValue)
                    {
                        var aux = dmg.MinValue;
                        dmg.MinValue = dmg.MaxValue;
                        dmg.MaxValue = aux;
                    }
                    dmg.Value = (float)System.Math.Round(Random.Range(dmg.DefaultMinValue, dmg.DefaultMaxValue) * 100f) / 100f;
                }
                else
                {
                    dmg.MinValue = item.Damage[key].MinValue;
                    dmg.MaxValue = item.Damage[key].MaxValue;
                    dmg.Value = item.Damage[key].Value;
                }
                Damage.Add(key, dmg);
            }
        }

        Name = item.Name;
        Description = item.Description;
        Weight = item.Weight;

        if (createNew)
        {
            if (item.Durability != null)
            {
                Durability = item.Durability.GetMinMaxValues(item.Durability, true);
            }
            if (item.Quantity != null)
            {
                Quantity = item.GetQuantity(item.Quantity);
            }
            if (item.CriticalChance != null)
            {
                CriticalChance = item.CriticalChance.GetMinMaxValues(item.CriticalChance, true);
            }
            if (item.Range != null)
            {
                Range = item.Range.GetMinMaxValues(item.Range);
            }
            if (item.AttackRecoveryTime != null)
            {
                AttackRecoveryTime = item.AttackRecoveryTime.GetMinMaxValues(item.AttackRecoveryTime);
            }
            if (item.Speed != null)
            {
                Speed = item.Speed.GetMinMaxValues(item.Speed, true);
            }
            if (item.StaminaConsumption != null)
            {
                StaminaConsumption = item.StaminaConsumption.GetMinMaxValues(item.StaminaConsumption);
            }
            if (item.InventorySlots != null)
            {
                InventorySlots = item.InventorySlots.GetMinMaxValues(item.InventorySlots, true);
            }
            if (item.Regain != null)
            {
                Regain = item.Regain.GetMinMaxValues(item.Regain);
            }
            if (item.ChargeSpeed != null)
            {
                ChargeSpeed = item.ChargeSpeed.GetMinMaxValues(item.ChargeSpeed);
            }
        }
        else
        {
            if (item.Durability != null)
            {
                Durability = item.Durability.GetMinMaxCopy(item.Durability);
            }
            if (item.Quantity != null)
            {
                Quantity = item.Quantity.GetMinMaxCopy(item.Quantity);
            }
            if (item.CriticalChance != null)
            {
                CriticalChance = item.CriticalChance.GetMinMaxCopy(item.CriticalChance);
            }
            if (item.Range != null)
            {
                Range = item.Range.GetMinMaxCopy(item.Range);
            }
            if (item.AttackRecoveryTime != null)
            {
                AttackRecoveryTime = item.AttackRecoveryTime.GetMinMaxCopy(item.AttackRecoveryTime);
            }
            if (item.Speed != null)
            {
                Speed = item.Speed.GetMinMaxCopy(item.Speed);
            }
            if (item.StaminaConsumption != null)
            {
                StaminaConsumption = item.StaminaConsumption.GetMinMaxCopy(item.StaminaConsumption);
            }
            if (item.InventorySlots != null)
            {
                InventorySlots = item.InventorySlots.GetMinMaxCopy(item.InventorySlots);
            }
            if (item.Regain != null)
            {
                Regain = item.Regain.GetMinMaxCopy(item.Regain);
            }
            if (item.ChargeSpeed != null)
            {
                ChargeSpeed = item.ChargeSpeed.GetMinMaxCopy(item.ChargeSpeed);
            }
        }

        GameObject = item.GameObject;
        ItemCategory = item.ItemCategory;
        Type = item.Type;
        Projectile = item.Projectile;

        if (item.SpecialStats != null)
        {
            Dictionary<DamageTypeEnum, Damage> damageList = new Dictionary<DamageTypeEnum, Damage>();

            if (item.SpecialStats.Damage != null)
            {
                foreach (var key in item.SpecialStats.Damage.Keys)
                {
                    var dmg = new Damage()
                    {
                        Type = item.SpecialStats.Damage[key].Type,
                        MinValue = item.SpecialStats.Damage[key].MinValue,
                        MaxValue = item.SpecialStats.Damage[key].MaxValue
                    };
                    damageList.Add(key, dmg);
                }
            }

            var resistances = new Dictionary<DamageTypeEnum, float>();
            if (item.SpecialStats.Resistances != null)
            {
                foreach (var key in item.SpecialStats.Resistances.Keys)
                {
                    resistances.Add(key, item.SpecialStats.Resistances[key]);
                }
            }

            var attributes = item.SpecialStats.Attributes != null
                ? new Attributes()
                {
                    Charisma = item.SpecialStats.Attributes.Charisma,
                    Constitution = item.SpecialStats.Attributes.Constitution,
                    Dexterity = item.SpecialStats.Attributes.Dexterity,
                    Intelligence = item.SpecialStats.Attributes.Intelligence,
                    Strength = item.SpecialStats.Attributes.Strength
                }
                : new Attributes();
            SpecialStats = new SpecialStats(damageList, attributes, item.SpecialStats.Speed, item.SpecialStats.AttackSpeedPercent, item.SpecialStats.StaminaConsumptionPercent, item.SpecialStats.Stamina, item.SpecialStats.Health, resistances);
        }
    }

    public void LevelUp(int tier)
    {
        Tier = tier;

        if (Type == ItemTypeEnum.Armor)
        {
            IncreaseArmor();
            IncreaseDurability();
        }
        else if (Type == ItemTypeEnum.Consumable)
        {
            for (var j = 2; j <= Tier; j++)
            {
                Regain.MinValue *= Tier;
                Regain.MaxValue *= Tier;
            }
        }
        else if (Type == ItemTypeEnum.Feet)
        {
            IncreaseArmor();
            IncreaseDurability();

            for (var i = 2; i <= Tier; i++)
            {
                int rand = Random.Range(0, 5);
                Speed.Value += rand;
            }
        }
        else if (Type == ItemTypeEnum.Hands)
        {
            IncreaseArmor();
            IncreaseDurability();

            for (var i = 2; i <= Tier; i++)
            {
                int rand = Random.Range(0, 5);
                CriticalChance.Value += rand;
            }
        }
        else if (Type == ItemTypeEnum.Head)
        {
            IncreaseArmor();
            IncreaseDurability();

            float val = ChargeSpeed.Value;
            for (var i = 2; i <= Tier; i++)
            {
                float rand = Helpers.GetPercentOfValue(ChargeSpeed.Value, Random.Range(0, 5f));
                ChargeSpeed.Value -= rand;
            }
        }
        else if (Type == ItemTypeEnum.Hip)
        {
            for (var i = 2; i <= Tier; i++)
            {
                int rand = Random.Range(0, 3);
                InventorySlots.Value += rand;
            }
        }
        else if (Type == ItemTypeEnum.Neck)
        {
            if (Tier > 2)
            {
                int rand = Random.Range(0, 2);
                Slots = rand;
            }
        }
        else if (Type == ItemTypeEnum.Ring)
        {
            if (Tier > 2)
            {
                int rand = Random.Range(0, 2);
                Slots = rand;
            }
        }
        else if (Type == ItemTypeEnum.Shield)
        {
            IncreaseArmor();
            IncreaseDurability();
        }
        else if (Type == ItemTypeEnum.Weapon)
        {
            IncreaseDurability();

            foreach (var dmgType in Damage.Keys)
            {
                for (var i = 2; i <= Tier; i++)
                {
                    float maxVal = Damage[dmgType].MaxValue - ((Damage[dmgType].MaxValue - Damage[dmgType].MinValue) / 2);
                    float rand = Random.Range(0, maxVal);
                    Damage[dmgType].MinValue += rand;

                    rand = Random.Range(maxVal, Damage[dmgType].MaxValue);
                    Damage[dmgType].MinValue += rand;
                }

                if (Damage[dmgType].MinValue > Damage[dmgType].MaxValue)
                {
                    var aux = Damage[dmgType].MinValue;
                    Damage[dmgType].MinValue = Damage[dmgType].MaxValue;
                    Damage[dmgType].MaxValue = aux;
                }
            }

            for (var i = 2; i <= Tier; i++)
            {
                float rand = Helpers.GetPercentOfValue(StaminaConsumption.Value, Random.Range(0, 5f));
                StaminaConsumption.Value -= rand;
            }
        }

        SpecialStats = new SpecialStats();
        if (Tier > 2 && Type != ItemTypeEnum.Ring && Type != ItemTypeEnum.Neck)
        {
            SetSpecialStats(Tier - 2);
        }
        else if (Tier > 1 && (Type == ItemTypeEnum.Ring || Type == ItemTypeEnum.Neck))
        {
            SetSpecialStats(Tier - 1);
        }
    }

    private void IncreaseArmor()
    {
        foreach (var dmgType in Resistance.Keys)
        {
            // 0-4% per tier
            if (Resistance[dmgType].DefaultMinValue != 0 || Resistance[dmgType].DefaultMaxValue != 0)
            {
                int val = 0;
                for (var i = 2; i <= Tier; i++)
                {
                    int rand = Random.Range(0, 5);
                    if (Resistance[dmgType].Value != 0)
                    {
                        val += rand;
                    }
                }
                Resistance[dmgType].Value += val;
            }
        }
    }

    private void IncreaseDurability()
    {
        for (var i = 2; i <= Tier; i++)
        {
            int rand = Random.Range(0, 11);
            Durability.Value += rand;
            Durability.MaxValue += rand;
        }
    }

    private void SetSpecialStats(int nSpecialStats)
    {
        for (var i = 0; i < nSpecialStats; i++)
        {
            Enchantments++;
            int specialStatCategory = Random.Range(0, 10);
            switch (specialStatCategory)
            {
                // AttackSpeedPercent
                case 0:
                    for (var j = 0; j < nSpecialStats; j++)
                    {
                        int rand = Random.Range(0, 3);
                        SpecialStats.AttackSpeedPercent += rand;
                    }
                    break;
                // Attributes
                case 1:
                    SetSpecialAttributes();
                    break;
                // Damage
                case 2:
                    SetSpecialDamage(nSpecialStats);
                    break;
                // Health
                case 3:
                    for (var j = 0; j < nSpecialStats; j++)
                    {
                        int rand = Random.Range(0, 11);
                        SpecialStats.Health += rand;
                    }
                    break;
                // Mana
                case 4:
                    for (var j = 0; j < nSpecialStats; j++)
                    {
                        int rand = Random.Range(0, 4);
                        SpecialStats.Mana += rand;
                    }
                    break;
                case 5:
                    // Resistances
                    // 0-4% per tier
                    DamageTypeEnum dmgType = (DamageTypeEnum)Random.Range(0, 7);
                    int r = Random.Range(0, 5);
                    SpecialStats.Resistances[dmgType] += r;
                    break;
                // Speed
                case 6:
                    for (var j = 0; j < nSpecialStats; j++)
                    {
                        float rand = Helpers.RoundToTwoDecimals(Random.Range(0, 2f));
                        SpecialStats.Speed += rand;
                    }
                    break;
                // Stamina
                case 7:
                    for (var j = 0; j < nSpecialStats; j++)
                    {
                        float rand = Helpers.RoundToTwoDecimals(Random.Range(0, 7f));
                        SpecialStats.Stamina += rand;
                    }
                    break;
                case 8:
                    // Stamina Consumption
                    for (var j = 0; j < nSpecialStats; j++)
                    {
                        int rand = Random.Range(0, 3);
                        SpecialStats.StaminaConsumptionPercent += rand;
                    }
                    break;
            }
        }
    }

    private void SetSpecialAttributes()
    {
        int randAttr = Random.Range(0, 5);
        int rand = Random.Range(0, 3);
        switch (randAttr)
        {
            case 0:
                SpecialStats.Attributes.Strength += rand;
                break;
            case 1:
                SpecialStats.Attributes.Dexterity += rand;
                break;
            case 2:
                SpecialStats.Attributes.Constitution += rand;
                break;
            case 3:
                SpecialStats.Attributes.Intelligence += rand;
                break;
            case 4:
                SpecialStats.Attributes.Charisma += rand;
                break;
        }
    }

    private void SetSpecialDamage(int nSpecialStats)
    {
        DamageTypeEnum dmgType = (DamageTypeEnum)Random.Range(0, 7);

        float r = Helpers.RoundToTwoDecimals(Random.Range(0, 3f));
        SpecialStats.Damage[dmgType].MinValue += r;

        r = Helpers.RoundToTwoDecimals(Random.Range(0, 3f));
        SpecialStats.Damage[dmgType].MaxValue += r;

        if (SpecialStats.Damage[dmgType].MinValue > SpecialStats.Damage[dmgType].MaxValue)
        {
            var aux = SpecialStats.Damage[dmgType].MinValue;
            SpecialStats.Damage[dmgType].MinValue = SpecialStats.Damage[dmgType].MaxValue;
            SpecialStats.Damage[dmgType].MaxValue = aux;
        }
    }

    private MinMax GetQuantity(MinMax value)
    {
        MinMax mm = new MinMax();

        if (value != null)
        {
            mm.MaxValue = value.MaxValue;
            mm.Value = Random.Range(1, (int)mm.MaxValue + 1);
        }

        return mm;
    }

    public void RegainHealth(float health, Stats stats)
    {
        Debug.Log("Health to regain: " + health);
        Debug.Log("Health Current: " + stats.Health + "/" + stats.MaxHealth);
        Debug.Log("Health Total: " + stats.TotalHealth + "/" + stats.TotalMaxHealth);
        stats.Health += health;
        if (stats.Health > stats.MaxHealth)
        {
            stats.Health = stats.MaxHealth;
        }
        stats.TotalHealth += health;
        if (stats.TotalHealth > stats.TotalMaxHealth)
        {
            stats.TotalHealth = stats.TotalMaxHealth;
        }
    }

    public void RegainStamina(float stamina, Stats stats)
    {
        Debug.Log("Stamina to regain: " + stamina);
        Debug.Log("Stamina Current: " + stats.Stamina + "/" + stats.MaxStamina);
        Debug.Log("Stamina Total: " + stats.TotalStamina + "/" + stats.TotalMaxStamina);
        stats.Stamina += stamina;
        if (stats.Stamina > stats.MaxStamina)
        {
            stats.Stamina = stats.MaxStamina;
        }
        stats.TotalStamina += stamina;
        if (stats.TotalStamina > stats.TotalMaxStamina)
        {
            stats.TotalStamina = stats.TotalMaxStamina;
        }
    }

    public void RegainMana(float mana, Stats stats)
    {
        Debug.Log("Mana to regain: " + mana);
        Debug.Log("Mana Current: " + stats.Mana + "/" + stats.MaxMana);
        Debug.Log("Mana Total: " + stats.TotalStamina + "/" + stats.TotalMaxMana);
        stats.Mana += mana;
        if (stats.Mana > stats.MaxMana)
        {
            stats.Mana = stats.MaxMana;
        }
        stats.TotalMana += mana;
        if (stats.TotalMana > stats.TotalMaxMana)
        {
            stats.TotalMana = stats.TotalMaxMana;
        }
    }
}