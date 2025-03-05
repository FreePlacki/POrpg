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
            // TODO: do all items affect attributes or only equipped ones?
            var result = new Attributes(new());
            foreach (var item in Backpack.Items)
            {
                if (item.Attributes == null) continue;
                result += item.Attributes;
            }
            
            result += Equipment.LeftHand?.Attributes;
            result += Equipment.RightHand?.Attributes;
            
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
            if (fromItem?.IsTwoHanded == true)
            {
                if (Equipment.LeftHand != null)
                    this[from] = Equipment.LeftHand;
                else RemoveAt(from);
                Equipment.LeftHand = fromItem;
                if (Equipment.RightHand != null)
                    new EquipmentSlot(EquipmentSlotType.RightHand).MoveToBackpack(this);
                return;
            }

            // putting an item in hand makes the two-handed weapon go back to backpack
            if (Equipment.LeftHand?.IsTwoHanded == true)
                new EquipmentSlot(EquipmentSlotType.LeftHand).MoveToBackpack(this);
        }

        if (this[to] == null)
        {
            this[to] = fromItem;
            RemoveAt(from);
        }
        else
        {
            (this[from], this[to]) = (this[to], this[from]);
        }
    }

    public void AppendToBackpack(Item item) => this[new BackpackSlot(Backpack.Items.Count)] = item;

    public Item? RemoveAt(InventorySlot slot) => slot.Remove(this);
}