namespace POrpg.Enemies.Decisions;

public abstract class Decision
{
    public abstract void Execute(Dungeon.Dungeon dungeon);
}