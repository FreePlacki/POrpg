using System.Text;
using POrpg.ConsoleHelpers;

namespace POrpg.Dungeon;

public class InstructionsBuilder : IDungeonBuilder<string>
{
    private bool _hasWeapons;
    private bool _hasItems;
    private bool _hasModifiedItems;
    private bool _hasModifiedWeapons;
    private bool _hasPotions;
    private bool _hasEnemies;
    private bool _hasMoney;
    private bool _hasWalls = true; // TODO

    public IDungeonBuilder<string> AddRandomChambers(int numChambers) => this;
    public IDungeonBuilder<string> AddCentralRoom() => this;
    public IDungeonBuilder<string> AddRandomPaths() => this;

    public IDungeonBuilder<string> AddModifiedUnusableItems(double probability = 0.07, int maxEffects = 3)
    {
        _hasModifiedItems = true;
        return this;
    }

    public IDungeonBuilder<string> AddWeapons(double probability = 0.15)
    {
        _hasWeapons = true;
        return this;
    }

    public IDungeonBuilder<string> AddModifiedWeapons(double probability = 0.1, int maxEffects = 3)
    {
        _hasModifiedWeapons = true;
        return this;
    }

    public IDungeonBuilder<string> AddUnusableItems(double probability = 0.07)
    {
        _hasItems = true;
        return this;
    }

    public IDungeonBuilder<string> AddPotions(double probability = 0.15)
    {
        _hasPotions = true;
        return this;
    }

    public IDungeonBuilder<string> AddEnemies(double probability = 0.15)
    {
        _hasEnemies = true;
        return this;
    }

    public IDungeonBuilder<string> AddMoney(double probability = 0.15)
    {
        _hasMoney = true;
        return this;
    }

    public string Build()
    {
        var buffer = new StringBuilder();

        if (_hasWeapons) buffer.AppendLine($"There are {new StyledText("Weapons", Styles.Weapon)}!");
        if (_hasItems) buffer.AppendLine($"There are {new StyledText("Items", Styles.Item)}!");
        if (_hasModifiedItems && _hasModifiedWeapons)
            buffer.AppendLine($"{new StyledText("Weapons", Styles.Weapon)}" +
                              $" and {new StyledText("Items", Styles.Item)}" +
                              $" can have {new StyledText("special effects", Styles.Effect)}!");
        else if (_hasModifiedItems)
            buffer.AppendLine($"{new StyledText("Items", Styles.Item)}" +
                              $" can have {new StyledText("special effects", Styles.Effect)}!");
        else if (_hasModifiedWeapons)
            buffer.AppendLine($"{new StyledText("Weapons", Styles.Weapon)}" +
                              $" can have {new StyledText("special effects", Styles.Effect)}!");
        if (_hasPotions) buffer.AppendLine($"There are {new StyledText("Potions", Styles.Potion)}!");
        if (_hasEnemies) buffer.AppendLine($"There are {new StyledText("Enemies", Styles.Enemy)}!");
        if (_hasMoney)
            buffer.AppendLine($"There are two kinds of money: " +
                              $"{new StyledText("Gold", Styles.Money)} " +
                              $"and {new StyledText("Coin", Styles.Money)}," +
                              $"\nthey don't take up space in your inventory.");
        if (_hasWalls) buffer.AppendLine("You can't go through Walls (\u2588).");
        buffer.AppendLine($"There can be {new StyledText("many", Styles.Stacked)} things on a single tile.");

        return buffer.ToString();
    }
}