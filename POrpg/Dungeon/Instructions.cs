using System.Text;
using POrpg.ConsoleHelpers;

namespace POrpg.Dungeon;

public class Instructions
{
    private bool _hasWeapons;
    private bool _hasItems;
    private bool _hasPotions;
    private bool _hasEnemies;
    private bool _hasMoney;
    private bool _hasWalls;

    public void AddWeapons() => _hasWeapons = true;
    public void AddItems() => _hasItems = true;
    public void AddPotions() => _hasPotions = true;
    public void AddEnemies() => _hasEnemies = true;
    public void AddMoney() => _hasMoney = true;
    public void AddWalls() => _hasWalls = true;

    public string Build()
    {
        var buffer = new StringBuilder();

        if (_hasWeapons) buffer.AppendLine($"There are {new StyledText("Weapons", Styles.Weapon).Text}!");
        if (_hasItems) buffer.AppendLine($"There are  {new StyledText("Items", Styles.Item).Text}!");
        if (_hasPotions) buffer.AppendLine($"There are  {new StyledText("Potions", Styles.Potion).Text}!");
        if (_hasEnemies) buffer.AppendLine($"There are  {new StyledText("Enemies", Styles.Enemy).Text}!");
        if (_hasMoney)
            buffer.AppendLine($"There are two kinds of money: " +
                              $"{new StyledText("Gold", Styles.Money).Text} " +
                              $"and {new StyledText("Coin", Styles.Money).Text}" +
                              $"\nThey don't take up space in your inventory.");
        if (_hasWalls) buffer.AppendLine("You can't go through Walls (\u2588).");

        return buffer.ToString();
    }
}