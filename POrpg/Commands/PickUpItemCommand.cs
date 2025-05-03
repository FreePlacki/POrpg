using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public class PickUpItemCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly int _playerId;

    public PickUpItemCommand(ConsoleView view, Dungeon.Dungeon dungeon)
    {
        _playerId = view.PlayerId;
        _dungeon  = dungeon;
    }

    public string? Description { get; private set; }

    public void Execute()
    {
        var item = _dungeon.TryPickUpItem(_playerId);
        if (item != null)
            Description = $"Picked up {item.Name}";
    }
}