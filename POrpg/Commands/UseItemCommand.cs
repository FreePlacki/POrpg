namespace POrpg.Commands;

public class UseItemCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    public UseItemCommand(Dungeon.Dungeon dungeon)
    {
        _dungeon = dungeon;
    }
    
    public string? Description { get; private set; }

    public void Execute()
    {
        var item = _dungeon.TryUseItem();
        if (item != null)
            Description = $"Used {item.Name}";
    }
}