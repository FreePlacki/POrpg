namespace POrpg.Commands;

public class UseItemCommand : ICommand
{
    public string? Description { get; private set; }
    public bool AdvancesTurn { get; private set; } = true;

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        var item = dungeon.TryUseItem(playerId);
        if (item == null)
        {
            AdvancesTurn = false;
        }
        else
        {
            Description = $"Used {item.Name}";
            dungeon.Players[playerId].SelectedSlot = null;
        }
    }
}