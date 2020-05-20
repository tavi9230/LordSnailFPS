public class Damage
{
    public DamageTypeEnum Type { get; set; }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    public float Value { get; set; }

    public float DefaultMinValue { get; set; }
    public float DefaultMaxValue { get; set; }
}