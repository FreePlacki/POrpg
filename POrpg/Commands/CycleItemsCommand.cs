namespace POrpg.Commands;

public class CycleItemsCommand : ICommand
{
    private readonly bool _reverse;

    public CycleItemsCommand(bool reverse = false)
    {
        _reverse = reverse;
    }

    public bool AdvancesTurn => false;

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        dungeon.CycleItems(_reverse, playerId);
    }
}