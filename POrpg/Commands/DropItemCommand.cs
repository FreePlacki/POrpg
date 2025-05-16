namespace POrpg.Commands;

public class DropItemCommand : ICommand
{
    public string? Description { get; private set; }

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        var item = dungeon.TryDropItem(playerId);
        if (item != null)
            Description = $"Dropped {item.Name}";
    }
}