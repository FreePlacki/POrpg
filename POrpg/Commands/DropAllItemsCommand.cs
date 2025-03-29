namespace POrpg.Commands;

public class DropAllItemsCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;

    public DropAllItemsCommand(Dungeon.Dungeon dungeon)
    {
        _dungeon = dungeon;
    }

    public string? Description { get; private set; }
    public void Execute()
    {
        if (_dungeon.TryDropAllItems())
            Description = "Dropped all items";
    }
}