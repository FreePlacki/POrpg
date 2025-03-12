using POrpg.Inventory;

namespace POrpg.Items.Effects;

public class Unequippable : Effect
{
    public Unequippable(Item item) : base(item)
    {
    }

    public override string Name => $"{Item.Name} (Unequippable)";
    public override EquipmentSlotType EquipmentSlotType => EquipmentSlotType.None;
}