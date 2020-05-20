public class RayFinder
{
    public bool IsSeen { get; set; }
    public string Name { get; set; }

    public RayFinder(bool isSeen, string name)
    {
        IsSeen = isSeen;
        Name = name;
    }
}