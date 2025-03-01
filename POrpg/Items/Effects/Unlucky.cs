namespace POrpg.Items.Effects;

public class Unlucky : Effect, IItem
{
    public Unlucky(IItem item) : base(item)
    {
    }

    public override string Name => $"{Item.Name} (Unlucky)";
    Attributes? IItem.Attributes => new Attributes(new Dictionary<Attribute, int> { { Attribute.Luck, -5}}) + Item.Attributes;
}