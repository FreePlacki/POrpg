using POrpg.Dungeon;

namespace POrpg.Commands;

public class MovePlayerCommand : ICommand
{
    private readonly Position _direction;

    public MovePlayerCommand(Position direction)
    {
        _direction = direction;
    }

    public bool AdvancesTurn { get; private set; }

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        if (dungeon.TryMovePlayer(_direction, playerId))
            AdvancesTurn = true;
    }
}