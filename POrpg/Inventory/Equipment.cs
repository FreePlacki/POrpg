using POrpg.Items;

namespace POrpg.Inventory;

public enum EquipmentSlotType
{
    LeftHand,
    RightHand,
    BothHands
}

public class Equipment
{
    public IItem? LeftHand { get; set; }
    public IItem? RightHand { get; set; }

    public void SetItem(EquipmentSlotType slot, IItem? item)
    {
        switch (slot)
        {
            case EquipmentSlotType.LeftHand or EquipmentSlotType.BothHands:
                LeftHand = item;
                if (item?.IsTwoHanded == true) RightHand = null;
                break;
            case EquipmentSlotType.RightHand:
                RightHand = item;
                if (item?.IsTwoHanded == true)
                {
                    LeftHand = item;
                    RightHand = null;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
        }
    }

    public IItem? GetItem(EquipmentSlotType slot) => slot switch
    {
        EquipmentSlotType.LeftHand or EquipmentSlotType.BothHands => LeftHand,
        EquipmentSlotType.RightHand => RightHand,
        _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
    };
}
public record EquipmentSlot : InventorySlot
{
    public EquipmentSlotType SlotType { get; }

    public EquipmentSlot(EquipmentSlotType slotType)
    {
        SlotType = slotType;
    }

    public override IItem? Get(Inventory inventory) =>
        inventory.Equipment.GetItem(SlotType);

    public override void Set(Inventory inventory, IItem? item) =>
        inventory.Equipment.SetItem(SlotType, item);

    public override IItem? Remove(Inventory inventory)
    {
        var item = Get(inventory);
        Set(inventory, null);
        return item;
    }

    public override bool IsValid(Inventory inventory) => true;

    public override InventorySlot Normalize(Inventory inventory)
    {
        if (inventory.Equipment.LeftHand?.IsTwoHanded == true)
        {
            return new EquipmentSlot(EquipmentSlotType.BothHands);
        }

        return this;
    }

    public override bool CanMoveToBackpack => true;

    public override void MoveToBackpack(Inventory inventory)
    {
        var item = Get(inventory);
        if (item == null) return;
        inventory.Backpack.Items.Add(item);
        Set(inventory, null);
    }
}
