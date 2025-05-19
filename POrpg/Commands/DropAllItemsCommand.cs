namespace POrpg.Commands;

public class DropAllItemsCommand : ICommand
{
    public string? Description { get; private set; }
    public bool AdvancesTurn => true;

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        if (dungeon.TryDropAllItems(playerId))
            Description = "Dropped all items";
    }
}