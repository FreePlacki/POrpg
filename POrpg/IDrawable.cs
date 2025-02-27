namespace POrpg;

public interface IDrawable
{
    string ToString();
    string Description { get; }
    bool IsPassable => true;
}