namespace POrpg.Items.Effects;

public abstract class Effect : Item
{
    protected readonly Item Item;

    protected Effect(Item item)
    {
        Item = item;
    }
}