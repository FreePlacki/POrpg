using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public class MoveToBackpackCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly int _playerId;

    public MoveToBackpackCommand(ConsoleView view, Dungeon.Dungeon dungeon)
    {
        _playerId = view.PlayerId;
        _dungeon  = dungeon;
    }

    public void Execute()
    {
        _dungeon.TryMoveToBackpack(_playerId);
    }
}