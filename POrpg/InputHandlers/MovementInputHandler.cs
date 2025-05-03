using POrpg.Commands;
using POrpg.ConsoleHelpers;

namespace POrpg.InputHandlers;

public class MovementInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleView view, Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.W or ConsoleKey.UpArrow => new MovePlayerCommand(view, dungeon, (0, -1)),
            ConsoleKey.S or ConsoleKey.DownArrow => new MovePlayerCommand(view, dungeon, (0, 1)),
            ConsoleKey.A or ConsoleKey.LeftArrow => new MovePlayerCommand(view, dungeon, (-1, 0)),
            ConsoleKey.D or ConsoleKey.RightArrow => new MovePlayerCommand(view, dungeon, (1, 0)),
            _ => NextHandler!.HandleInput(view, dungeon, keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("WSAD", "Move", UiLocation.Bottom);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}