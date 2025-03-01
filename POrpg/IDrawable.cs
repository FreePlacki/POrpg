namespace POrpg;

public interface IDrawable
{
    string Symbol { get; }
    string Name { get; }
    string? Description => null;
    bool IsPassable => true;
}