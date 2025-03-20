using POrpg.ConsoleHelpers;

namespace POrpg.Items;

public class Coin : Item
{
    public override string Symbol => new StyledText("C", Styles.Money).Text;
    public override string Name => "Coin";

    public override bool OnPickUp(Player player)
    {
        player.Coins += 1;
        return true;
    }
}