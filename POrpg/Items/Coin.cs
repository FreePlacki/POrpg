using POrpg.ConsoleHelpers;

namespace POrpg.Items;

public class Coin : IItem
{
    public string Symbol => new StyledText("C", Style.Yellow).Text;
    public string Name => "Coin";

    public bool OnPickUp(Player player)
    {
        player.Coins += 1;
        return true;
    }
}