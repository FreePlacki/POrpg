using POrpg.ConsoleHelpers;

namespace POrpg.Items;

public class Gold : IItem
{
    public string Symbol => new StyledText("G", Style.Yellow).Text;
    public string Name => "Gold";
    public void PickUp(Player player)
    {
        player.Gold += 1;
    }
}