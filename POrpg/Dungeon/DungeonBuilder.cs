namespace POrpg.Dungeon;

public abstract class DungeonBuilder
{
    protected readonly Dungeon Dungeon;
    protected readonly Random _rng = new();

    protected DungeonBuilder(InitialDungeonState initialState, int width, int height)
    {
        Dungeon = new Dungeon(initialState, width, height);
    }

    public abstract DungeonBuilder AddRandomPaths(int numPaths);
    public abstract DungeonBuilder AddRandomChambers(int numChambers);
    public abstract DungeonBuilder AddCentralRoom();
    public abstract DungeonBuilder AddUnusableItems(double probability = 0.07, int maxEffects = 0);
    public abstract DungeonBuilder AddWeapons(double probability = 0.15, int maxEffects = 0);
    public abstract DungeonBuilder AddMoney(double probability = 0.15);
    public abstract Dungeon Build();
}