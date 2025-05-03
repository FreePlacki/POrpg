using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public class DropItemCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly int _playerId;
    
    public DropItemCommand(ConsoleView view, Dungeon.Dungeon dungeon)
    {
        _playerId = view.PlayerId;
        _dungeon = dungeon;
    }

    public string? Description { get; private set; }
    
    public void Execute()
    {
        var item = _dungeon.TryDropItem(_playerId);
        if (item != null)
            Description = $"Dropped {item.Name}";
    }
}