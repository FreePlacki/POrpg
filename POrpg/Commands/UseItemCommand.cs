namespace POrpg.Commands;

public class UseItemCommand : ICommand
{
    public string? Description { get; private set; }

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        var item = dungeon.TryUseItem(playerId);
        if (item != null)
            Description = $"Used {item.Name}";
    }
}