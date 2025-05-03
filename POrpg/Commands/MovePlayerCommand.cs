using POrpg.ConsoleHelpers;
using POrpg.Dungeon;

namespace POrpg.Commands;

public class MovePlayerCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly Position _direction;
    private readonly int _playerId;

    public MovePlayerCommand(ConsoleView view, Dungeon.Dungeon dungeon, Position direction)
    {
        _playerId  = view.PlayerId;
        _dungeon   = dungeon;
        _direction = direction;
    }

    public bool AdvancesTurn { get; private set; }

    public void Execute()
    {
        if (_dungeon.TryMovePlayer(_direction, _playerId))
            AdvancesTurn = true;
    }
}