namespace POrpg.Commands;

public class PickUpItemCommand : ICommand
{
    public string? Description { get; private set; }

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        var item = dungeon.TryPickUpItem(playerId);
        if (item != null)
            Description = $"Picked up {item.Name}";
    }
}