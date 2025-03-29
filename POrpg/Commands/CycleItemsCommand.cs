namespace POrpg.Commands;

public class CycleItemsCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly bool _reverse;

    public CycleItemsCommand(Dungeon.Dungeon dungeon, bool reverse = false)
    {
        _dungeon = dungeon;
        _reverse = reverse;
    }

    public bool AdvancesTurn => false;

    public void Execute()
    {
        _dungeon.CycleItems(_reverse);
    }
}