using POrpg.Commands;

namespace POrpg.InputHandlers;

public class GuardInputHandler : InputHandler
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo) =>
        new InvalidInputCommand();
}