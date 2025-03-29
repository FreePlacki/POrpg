namespace POrpg.Effects;

public class HealEffect : Effect
{
    public HealEffect(Player player, int? duration = null, string name = "Strength Effect")
        : base(player, duration ?? 0, name, duration == null)
    {
    }

    public override Attributes Attributes => new(new() { { Attribute.Health, 5 } });
}