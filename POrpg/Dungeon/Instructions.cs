using System.Text;
using POrpg.ConsoleHelpers;

namespace POrpg.Dungeon;

public class Instructions
{
    private bool _hasWeapons;
    private bool _hasItems;
    private bool _hasPotions;
    private bool _hasMoney;
    private bool _hasWalls;

    public void AddWeapons() => _hasWeapons = true;
    public void AddItems() => _hasItems = true;
    public void AddPotions() => _hasPotions = true;
    public void AddMoney() => _hasMoney = true;
    public void AddWalls() => _hasWalls = true;

    public string Build()
    {
        var buffer = new StringBuilder();

        if (_hasWeapons) buffer.AppendLine($"There are {new StyledText("Weapons", Style.Cyan).Text}!");
        if (_hasItems) buffer.AppendLine($"There are  {new StyledText("Items", Style.Blue).Text}!");
        if (_hasItems) buffer.AppendLine($"There are  {new StyledText("Potions", Style.Blue).Text}!");
        if (_hasMoney)
            buffer.AppendLine($"There are two kinds of money: " +
                              $"{new StyledText("Gold", Style.Yellow).Text} " +
                              $"and {new StyledText("Coin", Style.Yellow).Text}" +
                              $"\nThey don't take up space in your inventory.");
        if (_hasWalls) buffer.AppendLine($"You can't go through Walls (\u2588).");

        return buffer.ToString();
    }
}