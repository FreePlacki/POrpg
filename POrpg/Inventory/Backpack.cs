using POrpg.Items;

namespace POrpg.Inventory;

public class Backpack
{
    public List<Item> Items { get; } = [];
    public bool IsEmpty => Items.Count == 0;
}

public record BackpackSlot : InventorySlot
{
    public int SlotIndex { get; }

    public BackpackSlot(int slotIndex)
    {
        SlotIndex = slotIndex;
    }

    public override Item? Get(Inventory inventory)
    {
        return SlotIndex < inventory.Backpack.Items.Count
            ? inventory.Backpack.Items[SlotIndex]
            : null;
    }

    public override void Set(Inventory inventory, Item? item)
    {
        if (SlotIndex >= inventory.Backpack.Items.Count)
        {
            // unlimited backpack capacity?
            if (item != null)
                inventory.Backpack.Items.Add(item);
            return;
        }

        if (item == null)
        {
            inventory.Backpack.Items.RemoveAt(SlotIndex);
            return;
        }

        inventory.Backpack.Items[SlotIndex] = item;
    }

    public override Item? Remove(Inventory inventory)
    {
        var item = Get(inventory);
        Set(inventory, null);
        return item;
    }

    public override bool IsValid(Inventory inventory) => SlotIndex >= 0 && SlotIndex < inventory.Backpack.Items.Count;
}