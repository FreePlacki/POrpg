using POrpg.ConsoleHelpers;

namespace POrpg.Items.Potions;

public abstract class Potion : Item, IUsable
{
    private readonly int? _duration;

    protected Potion(int? duration)
    {
        _duration = duration;
    }

    public override string Symbol => new StyledText("P", Styles.Potion).Text;
    public abstract void Use(Player player);

    public override string Description => $"Duration: {_duration?.ToString() ?? "Permanent"}";
}