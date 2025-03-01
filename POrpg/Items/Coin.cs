using POrpg.ConsoleHelpers;

namespace POrpg.Items;

public class Coin : IItem
{
    public string Symbol => new StyledText("C", Style.Yellow).Text;
    public string Name => "Coin";

    public void PickUp(Player player)
    {
        player.Coins += 1;
    }
}