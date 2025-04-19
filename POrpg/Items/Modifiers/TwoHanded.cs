using POrpg.Inventory;

namespace POrpg.Items.Modifiers;

public class TwoHanded : Modifier
{
    public TwoHanded(Item item) : base(item)
    {
    }

    public override string Name => $"Two-Handed {Item.Name}";
    public override EquipmentSlotType EquipmentSlotType => EquipmentSlotType.BothHands;
}