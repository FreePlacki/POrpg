using POrpg.Items;

namespace POrpg.Inventory;

public class Backpack
{
    public List<Item> Items { get; } = [];
    public bool IsEmpty => Items.Count == 0;
    public bool IsFull => Items.Count >= 9;
}

public record BackpackSlot : InventorySlot
{
    private readonly int _slotIndex;

    public BackpackSlot(int slotIndex)
    {
        _slotIndex = slotIndex;
    }

    public override Item? Get(Inventory inventory)
    {
        return _slotIndex < inventory.Backpack.Items.Count
            ? inventory.Backpack.Items[_slotIndex]
            : null;
    }

    public override void Set(Inventory inventory, Item? item)
    {
        if (_slotIndex >= inventory.Backpack.Items.Count)
        {
            if (item != null)
                inventory.Backpack.Items.Add(item);
            return;
        }

        if (item == null)
        {
            inventory.Backpack.Items.RemoveAt(_slotIndex);
            return;
        }

        inventory.Backpack.Items[_slotIndex] = item;
    }

    public override Item? Remove(Inventory inventory)
    {
        var item = Get(inventory);
        Set(inventory, null);
        return item;
    }

    public override bool IsValid(Inventory inventory) => _slotIndex >= 0 && _slotIndex < inventory.Backpack.Items.Count;
}