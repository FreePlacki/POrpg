using System.Text.Json.Serialization;
using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public interface ICommand
{
    void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
    }

    void Execute(ConsoleView view)
    {
    }

    [JsonInclude] string? Description => null;
    [JsonInclude] bool AdvancesTurn => Description != null;
}