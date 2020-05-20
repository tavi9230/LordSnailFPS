using UnityEngine;

public class MinMax
{
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    public float Value { get; set; }
    public float DefaultMinValue { get; set; }
    public float DefaultMaxValue { get; set; }

    public MinMax GetMinMaxValues(MinMax value, bool isInt = false)
    {
        MinMax mm = new MinMax();

        if (value != null)
        {
            mm.DefaultMaxValue = value.DefaultMaxValue;
            mm.DefaultMinValue = value.DefaultMinValue;
            mm.MaxValue = value.DefaultMaxValue;
            mm.MinValue = value.DefaultMinValue;
            if (isInt)
            {
                mm.Value = Random.Range((int)mm.DefaultMinValue, (int)mm.DefaultMaxValue + 1);
                mm.MaxValue = mm.Value;
                return mm;
            }

            mm.Value = Helpers.RoundToTwoDecimals(Random.Range(mm.DefaultMinValue, mm.DefaultMaxValue));
        }
        return mm;
    }

    public MinMax GetMinMaxCopy(MinMax value)
    {
        if (value != null)
        {
            return new MinMax()
            {
                DefaultMaxValue = value.DefaultMaxValue,
                DefaultMinValue = value.DefaultMinValue,
                MaxValue = value.MaxValue,
                MinValue = value.MinValue,
                Value = value.Value,
            };
        }
        return new MinMax();
    }
}