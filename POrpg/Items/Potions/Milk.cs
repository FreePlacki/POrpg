namespace POrpg.Items.Potions;

public class Milk : Potion
{
    public Milk() : base(null)
    {
    }

    public override string Name => "Milk";

    public override void Use(Dungeon.Dungeon dungeon, int playerId)
    {
        dungeon.Players[playerId].Effects.Clear();
    }
}