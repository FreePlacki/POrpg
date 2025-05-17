using POrpg.ConsoleUtils;

namespace POrpg.Items;

public class Coin : Item
{
    public override string Symbol => new StyledText("C", Styles.Money).ToString();
    public override string Name => "Coin";

    public override bool OnPickUp(Player player)
    {
        player.Coins += 1;
        return true;
    }
}