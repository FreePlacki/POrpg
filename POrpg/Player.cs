namespace POrpg;

public class Player : IDrawable
{
    public override string ToString() => "¶";
    public string Description => "Player";

    public Position Position;

    public int Strength = 10;
    public int Dexterity = 11;
    public int Health = 12;
    public int Luck = 13;
    public int Aggression = 14;
    public int Wisdom = 15;
    
    public Player(Position position) => Position = position;
}