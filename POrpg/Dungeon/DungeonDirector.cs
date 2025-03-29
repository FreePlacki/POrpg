namespace POrpg.Dungeon;

public class DungeonDirector
{
    public T Build<T>(IDungeonBuilder<T> builder)
    {
        return builder.AddCentralRoom()
            // .AddRandomChambers(2)
            .AddRandomPaths()
            .AddMoney()
            .AddUnusableItems()
            .AddWeapons()
            .AddModifiedWeapons()
            .AddModifiedUnusableItems()
            .AddPotions(probability: 0.5)
            .AddEnemies(probability: 0.05)
            .Build();
    }
}