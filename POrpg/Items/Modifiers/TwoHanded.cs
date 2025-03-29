using POrpg.Inventory;

namespace POrpg.Items.Effects;

public class TwoHanded : Modifier
{
    public TwoHanded(Item item) : base(item)
    {
    }

    public override string Name => $"Two-Handed {Item.Name}";
    public override EquipmentSlotType EquipmentSlotType => EquipmentSlotType.BothHands;
}