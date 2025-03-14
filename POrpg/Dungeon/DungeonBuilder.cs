namespace POrpg.Dungeon;

public abstract class DungeonBuilder
{
    protected Dungeon Dungeon;
    protected DungeonBuilder(InitialDungeonState initialState, int width, int height)
    {
        Dungeon = new Dungeon(initialState, width, height);
    }
    public abstract DungeonBuilder AddRandomPaths(int numPaths);
    public abstract DungeonBuilder AddRandomChambers(int numChambers);
    public abstract DungeonBuilder AddCentralRoom();
    public abstract Dungeon Build();
}