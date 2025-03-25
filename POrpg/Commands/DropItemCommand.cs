namespace POrpg.Commands;

public class DropItemCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    public DropItemCommand(Dungeon.Dungeon dungeon)
    {
        _dungeon = dungeon;
    }

    public string? Description { get; private set; }
    
    public void Execute()
    {
        var item = _dungeon.TryDropItem();
        if (item != null)
            Description = $"Dropped {item.Name}.";
    }
}