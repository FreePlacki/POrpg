namespace POrpg.Items.Effects;

public abstract class Effect : IItem
{
    protected readonly IItem Item;

    public Effect(IItem item)
    {
        Item = item;
    }

    public char Symbol => Item.Symbol;
    public abstract string Name { get; }
    Attributes? IItem.Attributes => Item.Attributes;
    int? IItem.Damage => Item.Damage;
}