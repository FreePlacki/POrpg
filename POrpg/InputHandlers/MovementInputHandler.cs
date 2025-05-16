using POrpg.Commands;

namespace POrpg.InputHandlers;

public class MovementInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.W or ConsoleKey.UpArrow => new MovePlayerCommand((0, -1)),
            ConsoleKey.S or ConsoleKey.DownArrow => new MovePlayerCommand((0, 1)),
            ConsoleKey.A or ConsoleKey.LeftArrow => new MovePlayerCommand((-1, 0)),
            ConsoleKey.D or ConsoleKey.RightArrow => new MovePlayerCommand((1, 0)),
            _ => NextHandler!.HandleInput(keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("WSAD", "Move", UiLocation.Bottom);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}