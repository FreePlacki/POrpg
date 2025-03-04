namespace POrpg.Items.Effects;

public abstract class Effect : Item
{
    protected readonly Item Item;

    protected Effect(Item item)
    {
        Item = item;
    }

    public override string Symbol => Item.Symbol;
    public override Attributes? Attributes => Item.Attributes;
    public override int? Damage => Item.Damage;
    public override bool IsTwoHanded => Item.IsTwoHanded;
}