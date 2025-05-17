namespace POrpg.Commands;

public class CycleItemsCommand : ICommand
{
    public bool Reverse { get; }

    public CycleItemsCommand(bool reverse = false)
    {
        Reverse = reverse;
    }

    public bool AdvancesTurn => false;

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        dungeon.CycleItems(Reverse, playerId);
    }
}