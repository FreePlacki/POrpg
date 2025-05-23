using POrpg.Dungeon;

namespace POrpg.Enemies.Behaviours;

public abstract record Decision;

public sealed record StayDecision : Decision;

public sealed record MoveDecision(Position Direction) : Decision;

public sealed record AttackDecition(Player player) : Decision;

public interface IBehaviour
{
    Decision ComputeAction(Position position, Dungeon.Dungeon dungeon);
}