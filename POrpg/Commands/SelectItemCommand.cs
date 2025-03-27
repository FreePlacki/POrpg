using POrpg.Inventory;

namespace POrpg.Commands;

public class SelectItemCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly InventorySlot _selectedSlot;

    public SelectItemCommand(Dungeon.Dungeon dungeon, InventorySlot selectedSlot)
    {
        _dungeon = dungeon;
        _selectedSlot = selectedSlot;
    }

    public void Execute()
    {
        _dungeon.TrySelectItem(_selectedSlot);
    }
}