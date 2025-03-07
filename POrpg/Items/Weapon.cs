using POrpg.ConsoleHelpers;
using POrpg.Inventory;

namespace POrpg.Items;

public abstract class Weapon : Item
{
    public override string Symbol => new StyledText("W", Style.Cyan).Text;
    public override EquipmentSpace EquipmentSpace => EquipmentSpace.SingleHand;
}