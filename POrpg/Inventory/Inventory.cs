using POrpg.Items;

namespace POrpg.Inventory;

// the slots are records so that we can use structural ==
public abstract record InventorySlot
{
    public abstract Item? Get(Inventory inventory);
    public abstract void Set(Inventory inventory, Item? item);
    public abstract Item? Remove(Inventory inventory);

    public abstract bool IsValid(Inventory inventory);

    // ex. if the left hand contains a two-handed item the slot type is actually BothHands
    public virtual InventorySlot Normalize(Inventory inventory) => this;
    public virtual bool CanMoveToBackpack => false;

    public virtual void MoveToBackpack(Inventory slot)
    {
    }
}

public class Inventory
{
    public Equipment Equipment { get; } = new();
    public Backpack Backpack { get; } = new();

    public Item? this[InventorySlot slot]
    {
        get => slot.Get(this);
        set => slot.Set(this, value);
    }

    public Attributes? TotalAttributes
    {
        get
        {
            var result = new Attributes(new());

            result += Equipment.LeftHand?.Attributes;
            result += Equipment.RightHand?.Attributes;
            result += Equipment.BothHands?.Attributes;

            return result.IsEmpty ? null : result;
        }
    }

    public void Swap(InventorySlot from, InventorySlot to)
    {
        var fromItem = this[from];
        if (to == new EquipmentSlot(EquipmentSlotType.BothHands) ||
            to == new EquipmentSlot(EquipmentSlotType.LeftHand) ||
            to == new EquipmentSlot(EquipmentSlotType.RightHand))
        {
            if (fromItem?.EquipmentSlotType == EquipmentSlotType.BothHands)
            {
                new EquipmentSlot(EquipmentSlotType.LeftHand).MoveToBackpack(this);
                new EquipmentSlot(EquipmentSlotType.RightHand).MoveToBackpack(this);

                to = new EquipmentSlot(EquipmentSlotType.BothHands);
                this[to] = fromItem;
                this[from] = null;
                return;
            }

            if (Equipment.BothHands != null)
            {
                new EquipmentSlot(EquipmentSlotType.BothHands).MoveToBackpack(this);
                this[to] = fromItem;
                this[from] = null;
                return;
            }
        }

        (this[from], this[to]) = (this[to], this[from]);
    }

    public void AppendToBackpack(Item item) => this[new BackpackSlot(Backpack.Items.Count)] = item;

    public Item? RemoveAt(InventorySlot slot) => slot.Remove(this);
}