using POrpg.Inventory;

namespace POrpg.Commands;

public class SelectItemCommand : ICommand
{
    private readonly InventorySlot _selectedSlot;

    public SelectItemCommand(InventorySlot selectedSlot)
    {
        _selectedSlot = selectedSlot;
    }

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        dungeon.TrySelectItem(_selectedSlot, playerId);
    }
}