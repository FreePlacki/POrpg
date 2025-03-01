namespace POrpg;

public interface IDrawable
{
    string Symbol { get; }
    string Name { get; }
    bool IsPassable => true;
}