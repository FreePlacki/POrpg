using POrpg.Commands;
using POrpg.ConsoleHelpers;

namespace POrpg.InputHandlers;

public class GuardInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleView view, Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo) =>
        new MessageCommand("Invalid input");

    public override IEnumerable<InputHint> GetHints()
    {
        yield break;
    }
}