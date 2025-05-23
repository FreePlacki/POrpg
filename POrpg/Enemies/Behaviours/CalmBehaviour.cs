namespace POrpg.Enemies.Behaviours;

public class CalmBehaviour : IBehaviour
{
    public Decision ComputeAction(Dungeon.Dungeon dungeon) => new Stay();
}