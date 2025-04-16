namespace POrpg.Enemies;

public abstract class Enemy : IDrawable
{
    public abstract string Symbol { get; }
    public abstract string Name { get; }
    public string Description => $"Damage: {Damage}";

    public abstract int Damage { get; }
    public abstract int Health { get; }
    public abstract int Armor { get; }
}