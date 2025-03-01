using POrpg.ConsoleHelpers;

namespace POrpg;

public class Player : IDrawable
{
    public string Symbol => new StyledText("¶", Style.Magenta).Text;
    public string Name => "Player";

    public Position Position;

    public readonly Attributes Attributes = new(
        new()
        {
            { Attribute.Strength, 15 },
            { Attribute.Dexterity, 8 },
            { Attribute.Health, 3 },
            { Attribute.Luck, -13 },
            { Attribute.Aggression, 10 },
            { Attribute.Wisdom, 42 }
        });

    public Player(Position position) => Position = position;
}