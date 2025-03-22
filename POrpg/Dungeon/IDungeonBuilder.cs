namespace POrpg.Dungeon;

public interface IDungeonBuilder<out T>
{
    IDungeonBuilder<T> AddRandomChambers(int numChambers);
    IDungeonBuilder<T> AddCentralRoom();
    IDungeonBuilder<T> AddRandomPaths();
    IDungeonBuilder<T> AddUnusableItems(double probability = 0.07);
    IDungeonBuilder<T> AddModifiedUnusableItems(double probability = 0.07, int maxEffects = 3);
    IDungeonBuilder<T> AddWeapons(double probability = 0.07);
    IDungeonBuilder<T> AddModifiedWeapons(double probability = 0.10, int maxEffects = 3);
    IDungeonBuilder<T> AddPotions(double probability = 0.15);
    IDungeonBuilder<T> AddEnemies(double probability = 0.15);
    IDungeonBuilder<T> AddMoney(double probability = 0.15);
    T Build();
}