using System.Text.Json.Serialization;
using POrpg.Dungeon;

namespace POrpg.Commands;

public class MovePlayerCommand : ICommand
{
    [JsonInclude] public Position Direction { get; }

    public MovePlayerCommand(Position direction)
    {
        Direction = direction;
    }

    public bool AdvancesTurn { get; private set; }
    public string? Description { get; private set; }

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        if (dungeon.TryMovePlayer(Direction, playerId))
            AdvancesTurn = true;
    }
}