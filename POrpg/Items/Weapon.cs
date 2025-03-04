using POrpg.ConsoleHelpers;

namespace POrpg.Items;

public abstract class Weapon : Item
{
    public override string Symbol => new StyledText("W", Style.Cyan).Text;
}