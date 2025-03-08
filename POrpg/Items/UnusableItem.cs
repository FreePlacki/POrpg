namespace POrpg.Items;

public class UnusableItem : Item
{
    public override string Name { get; }
    
    public UnusableItem(string name)
    {
        Name = name;
    }
}