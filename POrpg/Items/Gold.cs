using POrpg.ConsoleHelpers;

namespace POrpg.Items;

public class Gold : Item
{
    public override string Symbol => new StyledText("G", Style.Yellow).Text;
    public override string Name => "Gold";

    public override bool OnPickUp(Player player)
    {
        player.Gold += 1;
        return true;
    }
}