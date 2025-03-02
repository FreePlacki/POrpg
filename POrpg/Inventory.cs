using POrpg.Items;

namespace POrpg;

public enum Hand
{
    Left,
    Right
}

public record InventorySlot
{
    public record HandSlot(Hand Hand) : InventorySlot;

    public record BackpackSlot(int Slot) : InventorySlot;
}

public class Inventory
{
    public IItem? LeftHand { get; private set; }
    public IItem? RightHand { get; private set; }

    public List<IItem> Backpack { get; } = [];

    public IItem? this[InventorySlot slot]
    {
        get => slot switch
        {
            InventorySlot.HandSlot(Hand.Left) => LeftHand,
            InventorySlot.HandSlot(Hand.Right) => RightHand,
            InventorySlot.BackpackSlot s => Backpack.ElementAtOrDefault(s.Slot),
            _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
        };
        private set
        {
            switch (slot)
            {
                case InventorySlot.HandSlot(Hand.Left):
                    LeftHand = value;
                    break;
                case InventorySlot.HandSlot(Hand.Right):
                    RightHand = value;
                    break;
                case InventorySlot.BackpackSlot(var slotIndex):
                    if (slotIndex < Backpack.Count)
                        Backpack[slotIndex] = value!;
                    else Backpack.Add(value!);
                    break;
            }
        }
    }

    public void Swap(InventorySlot to, InventorySlot from)
    {
        if (this[to] == null)
        {
            this[to] = this[from];
            RemoveAt(from);
        }
        else
        {
            (this[from], this[to]) = (this[to], this[from]);
        }
    }

    public void Append(IItem item)
    {
        this[new InventorySlot.BackpackSlot(Backpack.Count)] = item;
    }

    public IItem? RemoveAt(InventorySlot slot)
    {
        var item = this[slot];
        switch (slot)
        {
            case InventorySlot.HandSlot hand:
                this[hand] = null;
                break;
            case InventorySlot.BackpackSlot s:
                Backpack.RemoveAt(s.Slot);
                break;
        }

        return item;
    }

    public void MoveToBackpack(InventorySlot.HandSlot slot)
    {
        Backpack.Add(this[slot]!);
        this[slot] = null;
    }
}