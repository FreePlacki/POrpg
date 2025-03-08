using System.Diagnostics;
using POrpg.Items;

namespace POrpg.Inventory;

[Flags]
public enum EquipmentSlotType
{
    None = 0,
    LeftHand = 1,
    RightHand = 2,
    BothHands = 4
}

public class Equipment
{
    private readonly Dictionary<EquipmentSlotType, Item?> _equipment = new ()
        {
            {EquipmentSlotType.LeftHand, null},
            {EquipmentSlotType.RightHand, null},
            {EquipmentSlotType.BothHands, null}
        };

    public Item? LeftHand => _equipment[EquipmentSlotType.LeftHand];
    public Item? RightHand => _equipment[EquipmentSlotType.RightHand];
    public Item? BothHands => _equipment[EquipmentSlotType.BothHands];

    public Item? this[EquipmentSlotType slot]
    {
        get => _equipment[slot];
        set
        {
            Debug.Assert((value?.EquipmentSlotType & slot) != 0);
            _equipment[slot] = value;
        }
    }
}

public record EquipmentSlot : InventorySlot
{
    private readonly EquipmentSlotType _slotType;

    public EquipmentSlot(EquipmentSlotType slotType)
    {
        _slotType = slotType;
    }

    public override Item? Get(Inventory inventory) =>
        inventory.Equipment[_slotType];

    public override void Set(Inventory inventory, Item? item) =>
        inventory.Equipment[_slotType] = item;

    public override Item? Remove(Inventory inventory)
    {
        var item = Get(inventory);
        Set(inventory, null);
        return item;
    }

    public override bool IsValid(Inventory inventory) => true;

    public override InventorySlot Normalize(Inventory inventory)
    {
        return inventory.Equipment.BothHands != null ? new EquipmentSlot(EquipmentSlotType.BothHands) : this;
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