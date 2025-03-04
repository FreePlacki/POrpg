namespace POrpg.Items.Effects;

public class Powerful : Effect
{
    public Powerful(Item item) : base(item)
    {
    }

    public override string Name => $"{Item.Name} (Powerful)";
    public override int? Damage => Item.Damage + 5;
}