using POrpg.Dungeon;

namespace POrpg.Enemies.Behaviours;

public class CalmBehaviour : IBehaviour
{
    public Decision ComputeAction(Position position, Dungeon.Dungeon dungeon) => new StayDecision();
}