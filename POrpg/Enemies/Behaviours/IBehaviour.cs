using POrpg.Dungeon;

namespace POrpg.Enemies.Behaviours;

public abstract record Decision;

public sealed record Stay : Decision;

public sealed record Move(Position Direction) : Decision;

public sealed record Attack(Player player) : Decision;

public interface IBehaviour
{
    Decision ComputeAction(Dungeon.Dungeon dungeon);
}