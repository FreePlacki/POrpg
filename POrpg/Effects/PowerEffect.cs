namespace POrpg.Effects;

public class PowerEffect : Effect
{
    public PowerEffect(Player player, int? duration, string name = "Strength Effect")
        : base(player, duration ?? 0, name, duration == null)
    {
    }

    public override Attributes Attributes => new(new() { { Attribute.Strength, 5 } });
}