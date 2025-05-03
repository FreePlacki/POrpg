using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public class DropAllItemsCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly int _playerId;

    public DropAllItemsCommand(ConsoleView view,Dungeon.Dungeon dungeon)
    {
        _playerId = view.PlayerId;
        _dungeon  = dungeon;
    }

    public string? Description { get; private set; }
    public void Execute()
    {
        if (_dungeon.TryDropAllItems(_playerId))
            Description = "Dropped all items";
    }
}