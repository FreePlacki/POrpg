namespace POrpg.Items.Modifiers;

public class Unlucky : Modifier
{
    public Unlucky(Item item) : base(item)
    {
    }

    public override string Name => $"{Item.Name} (Unlucky)";
    public override Attributes? Attributes => new Attributes(new Dictionary<Attribute, int> { { Attribute.Luck, -5}}) + Item.Attributes;
}