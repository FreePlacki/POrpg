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
    public virtual InventorySlot Normalize(Inventory inventory, InventorySlot? selectedSlot) => this;

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

    // TODO refactor?
    public void Swap(InventorySlot from, InventorySlot to)
    {
        var fromItem = this[from.Normalize(this, from)];
        var toItem = this[to.Normalize(this, from)];

        switch (from)
        {
            case BackpackSlot:
                switch (to)
                {
                    case EquipmentSlot:
                        if (fromItem?.EquipmentSlotType == EquipmentSlotType.None) return;

                        if (toItem?.EquipmentSlotType == EquipmentSlotType.BothHands &&
                            fromItem?.EquipmentSlotType != EquipmentSlotType.BothHands)
                        {
                            this[new EquipmentSlot(EquipmentSlotType.BothHands)] = null;
                            this[to] = fromItem;
                            this[from] = toItem;
                            return;
                        }

                        if (fromItem?.EquipmentSlotType == EquipmentSlotType.BothHands &&
                            toItem?.EquipmentSlotType != EquipmentSlotType.BothHands)
                        {
                            this[from] = null;
                            new EquipmentSlot(EquipmentSlotType.LeftHand).MoveToBackpack(this);
                            new EquipmentSlot(EquipmentSlotType.RightHand).MoveToBackpack(this);
                            this[new EquipmentSlot(EquipmentSlotType.BothHands)] = fromItem;
                            return;
                        }

                        if (fromItem?.EquipmentSlotType == EquipmentSlotType.BothHands &&
                            toItem?.EquipmentSlotType == EquipmentSlotType.BothHands)
                        {
                            this[new EquipmentSlot(EquipmentSlotType.BothHands)] = fromItem;
                            this[from] = toItem;
                            return;
                        }

                        break;
                }

                break;
            case EquipmentSlot:
                switch (to)
                {
                    case BackpackSlot:
                        if (toItem?.EquipmentSlotType == EquipmentSlotType.None) return;
                        if (fromItem?.EquipmentSlotType == EquipmentSlotType.BothHands &&
                            toItem?.EquipmentSlotType != EquipmentSlotType.BothHands)
                        {
                            this[to] = fromItem;
                            this[new EquipmentSlot(EquipmentSlotType.BothHands)] = null;
                            this[new EquipmentSlot(EquipmentSlotType.LeftHand)] = toItem;
                            return;
                        }

                        if (fromItem?.EquipmentSlotType != EquipmentSlotType.BothHands &&
                            toItem?.EquipmentSlotType == EquipmentSlotType.BothHands)
                        {
                            this[new EquipmentSlot(EquipmentSlotType.BothHands)] = toItem;
                            this[new EquipmentSlot(EquipmentSlotType.LeftHand)] = null;
                            this[new EquipmentSlot(EquipmentSlotType.RightHand)] = null;
                            this[to] = fromItem;
                            return;
                        }

                        break;
                }

                break;
        }

        (this[from], this[to]) = (toItem, fromItem);
    }

    public void AppendToBackpack(Item item) => this[new BackpackSlot(Backpack.Items.Count)] = item;

    public Item? RemoveAt(InventorySlot slot) => slot.Remove(this);
}