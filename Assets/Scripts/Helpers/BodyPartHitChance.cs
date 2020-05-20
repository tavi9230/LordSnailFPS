public class BodyPartHitChance
{
    public InventoryLocationEnum Location { get; set; }
    public int MinChance { get; set; }
    public int MaxChance { get; set; }

    public BodyPartHitChance(InventoryLocationEnum location, int minChance, int maxChance)
    {
        Location = location;
        MinChance = minChance;
        MaxChance = maxChance;
    }
}