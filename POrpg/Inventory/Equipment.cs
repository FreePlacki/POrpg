using System.Diagnostics;
using System.Text.Json.Serialization;
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
    public Dictionary<EquipmentSlotType, Item?> Items { get; }

    public Equipment()
    {
        Items = new()
        {
            { EquipmentSlotType.LeftHand, null },
            { EquipmentSlotType.RightHand, null },
            { EquipmentSlotType.BothHands, null }
        };
    }

    [JsonConstructor]
    public Equipment(Dictionary<EquipmentSlotType, Item?> items)
    {
        Items = items;
    }

    public Item? LeftHand => Items[EquipmentSlotType.LeftHand];
    public Item? RightHand => Items[EquipmentSlotType.RightHand];
    public Item? BothHands => Items[EquipmentSlotType.BothHands];

    public Item? this[EquipmentSlotType slot]
    {
        get => Items[slot];
        set
        {
            Debug.Assert((value?.EquipmentSlotType & slot) != 0);
            Items[slot] = value;
        }
    }
}

public record EquipmentSlot : InventorySlot
{
    public EquipmentSlotType SlotType { get; }

    public EquipmentSlot(EquipmentSlotType slotType)
    {
        SlotType = slotType;
    }

    public override Item? Get(Inventory inventory) =>
        inventory.Equipment[SlotType];

    public override void Set(Inventory inventory, Item? item) =>
        inventory.Equipment[SlotType] = item;

    public override Item? Remove(Inventory inventory)
    {
        var item = Get(inventory);
        Set(inventory, null);
        return item;
    }

    public override bool IsValid(Inventory inventory) => true;

    public override InventorySlot Normalize(Inventory inventory, InventorySlot? selectedSlot)
    {
        // if (selectedSlot?.Get(inventory)?.EquipmentSlotType == EquipmentSlotType.BothHands ||
        //     (inventory.Equipment.BothHands != null && selectedSlot == null))
        if (inventory.Equipment.BothHands != null)
            return new EquipmentSlot(EquipmentSlotType.BothHands);
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