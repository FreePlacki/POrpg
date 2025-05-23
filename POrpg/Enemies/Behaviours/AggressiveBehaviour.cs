using POrpg.Dungeon;

namespace POrpg.Enemies.Behaviours;

public class AggressiveBehaviour : IBehaviour
{
    public Decision ComputeAction(Dungeon.Dungeon dungeon)
    {
        return new Move(new Position(0, -1));
    }
}