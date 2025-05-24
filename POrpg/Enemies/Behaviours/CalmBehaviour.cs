using POrpg.Dungeon;
using POrpg.Enemies.Decisions;

namespace POrpg.Enemies.Behaviours;

public class CalmBehaviour : IBehaviour
{
    public Decision ComputeAction(Position position, Dungeon.Dungeon dungeon) => new StayDecision();
}