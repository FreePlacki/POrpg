namespace POrpg.Commands;

public class PickUpItemCommand : ICommand
{
    public string? Description { get; private set; }
    public bool AdvancesTurn { get; private set; } = true;

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        var item = dungeon.TryPickUpItem(playerId);
        if (item == null)
            AdvancesTurn = false;
        else
            Description = $"Picked up {item.Name}";
    }
}