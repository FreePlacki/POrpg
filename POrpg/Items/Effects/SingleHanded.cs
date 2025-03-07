using POrpg.Inventory;

namespace POrpg.Items.Effects;

public class SingleHanded : Effect
{
    public SingleHanded(Item item) : base(item)
    {
    }

    public override string Name => Item.Name;
    public override EquipmentSpace EquipmentSpace => EquipmentSpace.SingleHand;
}