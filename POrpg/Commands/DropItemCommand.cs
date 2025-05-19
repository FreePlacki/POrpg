namespace POrpg.Commands;

public class DropItemCommand : ICommand
{
    public string? Description { get; private set; }
    public bool AdvancesTurn { get; private set; } = true;

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        var item = dungeon.TryDropItem(playerId);
        if (item == null)
            AdvancesTurn = false;
        else
            Description = $"Dropped {item.Name}";
    }
}