using POrpg.Inventory;

namespace POrpg.Items.Effects;

public class Unequippable : Modifier
{
    public Unequippable(Item item) : base(item)
    {
    }

    public override string Name => $"{Item.Name} (Unequippable)";
    public override EquipmentSlotType EquipmentSlotType => EquipmentSlotType.None;
}