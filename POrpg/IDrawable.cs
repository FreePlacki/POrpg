namespace POrpg;

public interface IDrawable
{
    char Symbol { get; }
    string Name { get; }
    string? Description => null;
    bool IsPassable => true;
}