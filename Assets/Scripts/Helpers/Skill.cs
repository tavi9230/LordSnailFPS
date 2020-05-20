using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill
{
    public string Id { get; set; }
    public int Tier { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public SkillTypeEnum Type { get; set; }
    public float AttackRecoveryTime { get; set; }
    public float ManaConsumption { get; set; }
    public GameObject GameObject { get; set; }
    public GameObject Projectile { get; set; }
    public float Range { get; set; }
    public Requirements Requirements { get; set; }
    public Dictionary<DamageTypeEnum, Damage> Damage { get; set; }
    public bool IsUnlocked { get; set; }
    public float ChargeSpeed { get; set; }

    public Skill()
    {
        Name = "New Skill";
        Tier = 0;
        IsUnlocked = false;
    }

    public Skill(Skill skill)
    {
        Id = skill.Id;
        Tier = skill.Tier;
        Name = skill.Name;
        Description = skill.Description;
        Type = skill.Type;
        AttackRecoveryTime = skill.AttackRecoveryTime;
        ManaConsumption = skill.ManaConsumption;
        GameObject = skill.GameObject;
        Projectile = skill.Projectile;
        Range = skill.Range;
        ChargeSpeed = skill.ChargeSpeed;
        IsUnlocked = false;

        Requirements = new Requirements()
        {
            Attributes = new Attributes()
            {
                Charisma = skill.Requirements.Attributes.Charisma,
                Constitution = skill.Requirements.Attributes.Constitution,
                Dexterity = skill.Requirements.Attributes.Dexterity,
                Intelligence = skill.Requirements.Attributes.Intelligence,
                Strength = skill.Requirements.Attributes.Strength
            },
            ExperienceCost = skill.Requirements.ExperienceCost
        };

        Damage = new Dictionary<DamageTypeEnum, Damage>();
        if (skill.Damage != null)
        {
            foreach (var key in skill.Damage.Keys)
            {
                var dmg = new Damage()
                {
                    Type = skill.Damage[key].Type,
                    MinValue = skill.Damage[key].MinValue,
                    MaxValue = skill.Damage[key].MaxValue
                };
                Damage.Add(key, dmg);
            }
        }
    }

    public void LevelUp(int level)
    {
        Tier = level + 1;
        if (Tier > 5)
        {
            Tier = 5;
        }
        foreach (var key in Damage.Keys)
        {
            Damage[key].MinValue = Damage[key].MinValue * level;
            Damage[key].MaxValue = Damage[key].MaxValue * level;
        }
    }
}