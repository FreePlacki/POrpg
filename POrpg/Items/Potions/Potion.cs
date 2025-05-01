using System.Text.Json.Serialization;
using POrpg.ConsoleHelpers;

namespace POrpg.Items.Potions;

public abstract class Potion : Item, IUsable
{
    [JsonInclude]
    protected readonly int? Duration;

    protected Potion(int? duration)
    {
        Duration = duration;
    }

    public override string Symbol => new StyledText(Name.First().ToString(), Styles.Potion).ToString();
    public abstract void Use(Player player);

    public override string? Description => Duration != null ? $"Duration: {Duration}" : null;
}