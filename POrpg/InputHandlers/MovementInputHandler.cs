using POrpg.Commands;

namespace POrpg.InputHandlers;

public class MovementInputHandler(InputHandler nextHandler) : InputHandler(nextHandler)
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.W or ConsoleKey.UpArrow => new MovePlayerCommand(dungeon, (0, -1)),
            ConsoleKey.S or ConsoleKey.DownArrow => new MovePlayerCommand(dungeon, (0, 1)),
            ConsoleKey.A or ConsoleKey.LeftArrow => new MovePlayerCommand(dungeon, (-1, 0)),
            ConsoleKey.D or ConsoleKey.RightArrow => new MovePlayerCommand(dungeon, (1, 0)),
            _ => NextHandler!.HandleInput(dungeon, keyInfo)
        };
    }
}