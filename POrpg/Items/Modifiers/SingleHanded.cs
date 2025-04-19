using POrpg.Inventory;

namespace POrpg.Items.Modifiers;

public class SingleHanded : Modifier
{
    public SingleHanded(Item item) : base(item)
    {
    }

    public override string Name => Item.Name;
    public override EquipmentSlotType EquipmentSlotType => EquipmentSlotType.LeftHand | EquipmentSlotType.RightHand;
}