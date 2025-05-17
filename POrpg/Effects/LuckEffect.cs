namespace POrpg.Effects;

public class LuckEffect : Effect
{
    public LuckEffect(Dungeon.Dungeon dungeon, Player player, int? duration = 4, string name = "Luck",
        bool isPermanent = false)
        : base(dungeon, player, duration, name, isPermanent)
    {
    }

    public override Attributes Attributes => new(new() { { Attribute.Luck, TurnsLeft } });
}