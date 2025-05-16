using POrpg.Commands;

namespace POrpg.InputHandlers;

public class GuardInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleKeyInfo keyInfo) =>
        new MessageCommand("Invalid input");

    public override IEnumerable<InputHint> GetHints()
    {
        yield break;
    }
}