namespace POrpg.Effects;

public class HealEffect : Effect
{
    public HealEffect(int playerId) : base(playerId)
    {
    }

    public override Attributes Attributes => new(new() { { Attribute.Health, 5 } });
    public override string Name => "Heal";
    public override int Duration { get; set; } = 0;
    public override bool IsPermanent => true;
}