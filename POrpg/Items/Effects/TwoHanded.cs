using POrpg.Inventory;

namespace POrpg.Items.Effects;

public class TwoHanded : Effect
{
    public TwoHanded(Item item) : base(item)
    {
    }

    public override string Name => $"{Item.Name} (Two-Handed)";
    public override EquipmentSpace EquipmentSpace => EquipmentSpace.TwoHand;
}