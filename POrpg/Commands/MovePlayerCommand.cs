using POrpg.Dungeon;

namespace POrpg.Commands;

public class MovePlayerCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly Position _direction;
    public MovePlayerCommand(Dungeon.Dungeon dungeon, Position direction)
    {
        _dungeon = dungeon;
        _direction = direction;
    }

    public bool AdvancesTurn { get; private set; }

    public void Execute()
    {
        if (_dungeon.TryMovePlayer(_direction))
            AdvancesTurn = true;
    }
}