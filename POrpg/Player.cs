namespace POrpg;

public class Player : IDrawable
{
    public char Symbol => '¶';
    public string Name => "Player";

    public Position Position;

    public readonly Attributes Attributes = new(
        new()
        {
            { Attribute.Strength, 10 },
            { Attribute.Dexterity, 11 },
            { Attribute.Health, 12 },
            { Attribute.Luck, 13 },
            { Attribute.Aggression, 14 },
            { Attribute.Wisdom, 15 }
        });

    public Player(Position position) => Position = position;
}