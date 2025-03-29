using POrpg.Commands;

namespace POrpg.InputHandlers;

public class UiInputHandler : InputHandler
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.C => new ClearConsoleCommand(),
            ConsoleKey.X => new QuitCommand(dungeon),
            _ when keyInfo.KeyChar == '?' => new ShowInstructionsCommand(),
            _ => NextHandler!.HandleInput(dungeon, keyInfo)
        };
    }
}