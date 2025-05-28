namespace POrpg.Dungeon;

public record struct Position(int X, int Y)
{
    public static implicit operator Position((int x, int y) p) => new() { X = p.x, Y = p.y };
    public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);
    public static Position operator -(Position a, Position b) => new(a.X - b.X, a.Y - b.Y);
    public static Position operator *(int x, Position a) => new(a.X * x, a.Y * x);
    
    public int Distance(Position position) => Math.Abs(position.X - X) + Math.Abs(position.Y - Y);
}
