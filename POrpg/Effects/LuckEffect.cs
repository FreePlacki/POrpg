namespace POrpg.Effects;

public class LuckEffect : Effect
{
    public LuckEffect(int playerId) : base(playerId)
    {
        Duration = 10;
    }

    public override Attributes Attributes => new(new() { { Attribute.Luck, Duration } });
    public override string Name => "Luck";
    public override int Duration { get; set; }
    public override bool IsPermanent => false;
}