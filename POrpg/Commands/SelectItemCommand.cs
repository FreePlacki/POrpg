using POrpg.Inventory;

namespace POrpg.Commands;

public class SelectItemCommand : ICommand
{
    public InventorySlot SelectedSlot { get; }

    public SelectItemCommand(InventorySlot selectedSlot)
    {
        SelectedSlot = selectedSlot;
    }

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        dungeon.TrySelectItem(SelectedSlot, playerId);
    }
}