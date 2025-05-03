using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public class CycleItemsCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly int _playerId;
    private readonly bool _reverse;

    public CycleItemsCommand(ConsoleView view, Dungeon.Dungeon dungeon, bool reverse = false)
    {
        _playerId = view.PlayerId;
        _dungeon = dungeon;
        _reverse = reverse;
    }

    public bool AdvancesTurn => false;

    public void Execute()
    {
        _dungeon.CycleItems(_reverse, _playerId);
    }
}