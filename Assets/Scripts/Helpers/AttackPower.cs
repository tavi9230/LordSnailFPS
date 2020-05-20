using System.Collections.Generic;

public class AttackPower
{

    public Dictionary<DamageTypeEnum, Damage> Damage { get; set; }
    public float AttackRecoveryTime { get; set; }
    public float StaminaConsumption { get; set; }
    public float ChargeAttackSpeed;
    public HotbarItem Attack;

    public AttackPower()
    {
        Attack = new HotbarItem();
        Damage = new Dictionary<DamageTypeEnum, Damage>()
        {
            {
                DamageTypeEnum.Bludgeoning,
                new Damage()
                {
                    Type = DamageTypeEnum.Bludgeoning,
                    MinValue = 0,
                    MaxValue = 0
                }
            },
            {
                DamageTypeEnum.Cold,
                new Damage()
                {
                    Type = DamageTypeEnum.Cold,
                    MinValue = 0,
                    MaxValue = 0
                }
            },
            {
                DamageTypeEnum.Fire,
                new Damage()
                {
                    Type = DamageTypeEnum.Fire,
                    MinValue = 0,
                    MaxValue = 0
                }
            },
            {
                DamageTypeEnum.Lightning,
                new Damage()
                {
                    Type = DamageTypeEnum.Lightning,
                    MinValue = 0,
                    MaxValue = 0
                }
            },
            {
                DamageTypeEnum.Piercing,
                new Damage()
                {
                    Type = DamageTypeEnum.Piercing,
                    MinValue = 0,
                    MaxValue = 0
                }
            },
            {
                DamageTypeEnum.Poison,
                new Damage()
                {
                    Type = DamageTypeEnum.Poison,
                    MinValue = 0,
                    MaxValue = 0
                }
            },
            {
                DamageTypeEnum.Slashing,
                new Damage()
                {
                    Type = DamageTypeEnum.Slashing,
                    MinValue = 0,
                    MaxValue = 0
                }
            }
        };
    }
}