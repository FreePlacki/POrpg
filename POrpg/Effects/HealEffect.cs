namespace POrpg.Effects;

public class HealEffect : Effect
{
    public HealEffect(Dungeon.Dungeon dungeon, Player player, int? duration = null, string name = "Heal")
        : base(dungeon, player, duration ?? 0, name, duration == null)
    {
    }

    public override Attributes Attributes => new(new() { { Attribute.Health, 5 } });
}