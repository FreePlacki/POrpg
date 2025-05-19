namespace POrpg.Effects;

public class PowerEffect : Effect
{
    public PowerEffect(int playerId) : base(playerId)
    {
        Duration = 5;
    }

    public override Attributes Attributes => new(new() { { Attribute.Strength, 5 } });
    public override string Name => "Strength";
    public override int Duration { get; set; }
    public override bool IsPermanent => false;
}