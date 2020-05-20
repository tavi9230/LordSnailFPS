public class HotbarItem
{
    public InventoryItem Item { get; set; }
    public Skill Skill { get; set; }
    public InventoryItem OldItem { get; set; }
    public Skill OldSkill { get; set; }

    public HotbarItem()
    {
        Item = null;
        Skill = null;
    }

    public void Replace(InventoryItem item, bool replaceOldHotbarItem = false)
    {
        ReplaceOldItem(replaceOldHotbarItem);
        Item = item;
        Skill = null;
    }

    public void Replace(Skill skill, bool replaceOldHotbarItem = false)
    {
        ReplaceOldItem(replaceOldHotbarItem);
        Item = null;
        Skill = skill;
    }

    private void ReplaceOldItem(bool replaceOldHotbarItem)
    {
        if (replaceOldHotbarItem)
        {
            if (Item != null)
            {
                OldItem = Item;
                OldSkill = null;
            }
            if (Skill != null)
            {
                OldItem = null;
                OldSkill = Skill;
            }
        }
    }
}