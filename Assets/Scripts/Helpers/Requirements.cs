public class Requirements
{
    public Attributes Attributes { get; set; }
    public float ExperienceCost { get; set; }

    public Requirements()
    {
        Attributes = new Attributes();
        ExperienceCost = 0;
    }
}