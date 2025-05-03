using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public class UseItemCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly int _playerId;
    
    public UseItemCommand(ConsoleView view,Dungeon.Dungeon dungeon)
    {
        _playerId = view.PlayerId;
        _dungeon = dungeon;
    }
    
    public string? Description { get; private set; }

    public void Execute()
    {
        var item = _dungeon.TryUseItem(_playerId);
        if (item != null)
            Description = $"Used {item.Name}";
    }
}