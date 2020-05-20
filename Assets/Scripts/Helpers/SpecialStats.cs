using System.Collections.Generic;
using UnityEngine;

public class SpecialStats
{
    public Dictionary<DamageTypeEnum, Damage> Damage { get; set; }
    public Attributes Attributes { get; set; }
    public float Speed { get; set; }
    public int AttackSpeedPercent { get; set; }
    public int StaminaConsumptionPercent { get; set; }
    public float Stamina { get; set; }
    public float Health { get; set; }
    public float Mana { get; set; }
    public Dictionary<DamageTypeEnum, float> Resistances { get; set; }

    public SpecialStats()
    {
        Damage = new Dictionary<DamageTypeEnum, Damage>()
        {
            {DamageTypeEnum.Piercing, new Damage() },
            {DamageTypeEnum.Slashing, new Damage() },
            {DamageTypeEnum.Bludgeoning, new Damage() },
            {DamageTypeEnum.Fire, new Damage() },
            {DamageTypeEnum.Cold, new Damage() },
            {DamageTypeEnum.Lightning, new Damage() },
            {DamageTypeEnum.Poison, new Damage() },
        };
        Attributes = new Attributes();
        Resistances = new Dictionary<DamageTypeEnum, float>()
        {
            {DamageTypeEnum.Piercing, 0 },
            {DamageTypeEnum.Slashing, 0 },
            {DamageTypeEnum.Bludgeoning, 0 },
            {DamageTypeEnum.Fire, 0 },
            {DamageTypeEnum.Cold, 0 },
            {DamageTypeEnum.Lightning, 0 },
            {DamageTypeEnum.Poison, 0},
        };
    }

    public SpecialStats(Dictionary<DamageTypeEnum, Damage> damage = null, Attributes attributes = null, float speed = 0, int attackSpeedPercent = 0, int staminaConsumptionPercent = 0,
        float stamina = 0, float health = 0, Dictionary<DamageTypeEnum, float> resistances = null, float mana = 0)
    {
        Damage = damage;
        Attributes = attributes;
        Speed = speed;
        AttackSpeedPercent = attackSpeedPercent;
        StaminaConsumptionPercent = staminaConsumptionPercent;
        Stamina = stamina;
        Health = health;
        Resistances = resistances;
        Mana = mana;
    }
}