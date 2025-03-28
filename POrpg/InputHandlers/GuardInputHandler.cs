using POrpg.Commands;

namespace POrpg.InputHandlers;

public class GuardInputHandler() : InputHandler(null)
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo) =>
        new InvalidInputCommand();
}