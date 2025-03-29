namespace POrpg.Items.Potions;

public class Milk : Potion
{
    public Milk() : base(null)
    {
    }

    public override string Name => "Milk";
    public override void Use(Player player)
    {
        player.Effects.Clear();
    }
}