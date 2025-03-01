using POrpg.ConsoleHelpers;

namespace POrpg.Items;

public class Sword : IWeapon
{
    public int Damage => 10;
    public string Symbol => new StyledText("S", Style.Cyan).Text;
    public string Name => "Sword";

    public void PickUp(Player player)
    {
        throw new NotImplementedException();
    }
}