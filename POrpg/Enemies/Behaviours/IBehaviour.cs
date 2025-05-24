using POrpg.Dungeon;
using POrpg.Enemies.Decisions;

namespace POrpg.Enemies.Behaviours;

public interface IBehaviour
{
    Decision ComputeAction(Position position, Dungeon.Dungeon dungeon);
}