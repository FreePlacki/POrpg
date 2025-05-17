using POrpg.ConsoleUtils;

namespace POrpg.Commands;

public interface ICommand
{
    void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
    }

    void Execute(ConsoleView view)
    {
    }

    string? Description => null;
    bool AdvancesTurn => Description != null;
}