using POrpg.Commands;

namespace POrpg.InputHandlers;

public abstract class InputHandler
{
    protected InputHandler? NextHandler;

    public void SetNext(InputHandler? handler) => NextHandler = handler;

    public abstract ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo);
}