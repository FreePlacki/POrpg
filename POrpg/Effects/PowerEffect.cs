namespace POrpg.Effects;

public class PowerEffect : Effect
{
    public PowerEffect(Dungeon.Dungeon dungeon, Player player, int? duration, string name = "Strength")
        : base(dungeon, player, duration ?? 0, name, duration == null)
    {
    }

    public override Attributes Attributes => new(new() { { Attribute.Strength, 5 } });
}