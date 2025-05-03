using POrpg.ConsoleHelpers;
using POrpg.Inventory;

namespace POrpg.Commands;

public class SelectItemCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly InventorySlot _selectedSlot;
    private readonly int _playerId;

    public SelectItemCommand(ConsoleView view, Dungeon.Dungeon dungeon, InventorySlot selectedSlot)
    {
        _playerId = view.PlayerId;
        _dungeon = dungeon;
        _selectedSlot = selectedSlot;
    }

    public void Execute()
    {
        _dungeon.TrySelectItem(_selectedSlot, _playerId);
    }
}