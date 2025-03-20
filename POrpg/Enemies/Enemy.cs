namespace POrpg.Enemies;

public abstract class Enemy : IDrawable
{
    public abstract string Symbol { get; }
    public abstract string Name { get; }
}