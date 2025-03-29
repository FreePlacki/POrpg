using POrpg.Commands;

namespace POrpg.InputHandlers;

public class UsableItemInputHandler : InputHandler
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.F => new UseItemCommand(dungeon),
            _ => NextHandler!.HandleInput(dungeon, keyInfo)
        };
    }
}