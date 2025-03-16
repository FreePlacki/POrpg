namespace POrpg.Dungeon;

public abstract class DungeonBuilder
{
    protected readonly Dungeon Dungeon;
    protected readonly Random Rng = new();
    protected readonly Instructions Instructions = new();
    protected readonly Position PlayerInitialPosition;

    protected DungeonBuilder(InitialDungeonState initialState, int width, int height, Position playerInitialPosition)
    {
        if (initialState != InitialDungeonState.Empty) Instructions.AddWalls();
        PlayerInitialPosition = playerInitialPosition;
        Dungeon = new Dungeon(initialState, width, height, playerInitialPosition);
    }

    public abstract DungeonBuilder AddRandomChambers(int numChambers);
    public abstract DungeonBuilder AddCentralRoom();
    public abstract DungeonBuilder AddRandomPaths();
    public abstract DungeonBuilder AddUnusableItems(double probability = 0.07, int maxEffects = 0);
    public abstract DungeonBuilder AddWeapons(double probability = 0.15, int maxEffects = 0);
    public abstract DungeonBuilder AddMoney(double probability = 0.15);
    public abstract Dungeon BuildDungeon();
    public abstract Instructions BuildInstructions();
}