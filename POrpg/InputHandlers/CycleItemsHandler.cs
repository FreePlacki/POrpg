using POrpg.Commands;

namespace POrpg.InputHandlers;

public class CycleItemsHandler(InputHandler nextHandler) : InputHandler(nextHandler)
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.OemPeriod => new CycleItemsCommand(dungeon),
            ConsoleKey.OemComma => new CycleItemsCommand(dungeon, reverse: true),
            _ => NextHandler!.HandleInput(dungeon, keyInfo)
        };
    }
}