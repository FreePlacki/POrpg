using POrpg.Dungeon;
using POrpg.Enemies.Behaviours;
using POrpg.Enemies.Decisions;

namespace POrpg.Enemies;

public abstract class Enemy : IDrawable
{
    public abstract string Symbol { get; }
    public abstract string Name { get; }
    public string Description => $"Damage: {Damage}\nArmor: {Armor}\nHealth: {Health}";

    public abstract int Damage { get; }
    public abstract int Health { get; set; }
    public abstract int Armor { get; }
    public abstract IBehaviour Behaviour { get; set; }

    public int DealDamage(int damage)
    {
        var dmg = Math.Max(0, damage - Armor);
        Health -= dmg;
        return dmg;
    }

    public Decision ComputeDecision(Position position, Dungeon.Dungeon dungeon) =>
        Behaviour.ComputeAction(position, dungeon);
}