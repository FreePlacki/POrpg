using POrpg.ConsoleHelpers;

namespace POrpg.Items.Potions;

public abstract class Potion : Item
{
    public override string Symbol => new StyledText("P", Style.Blue).Text;
}