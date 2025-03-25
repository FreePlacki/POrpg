using POrpg.Dungeon;

namespace POrpg.Commands;

public class PickUpItemCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    public PickUpItemCommand(Dungeon.Dungeon dungeon)
    {
        _dungeon = dungeon;
    }
    
    public string? Description { get; private set; }

    public void Execute()
    {
        var item = _dungeon.TryPickUpItem();
        if (item != null)
            Description = $"Picked up {item.Name}.";
    }
}