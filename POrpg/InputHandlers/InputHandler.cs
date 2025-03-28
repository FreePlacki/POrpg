using POrpg.Commands;

namespace POrpg.InputHandlers;

public abstract class InputHandler
{
    protected readonly InputHandler? NextHandler;

    protected InputHandler(InputHandler? nextHandler)
    {
        NextHandler = nextHandler;
    }
    
    public abstract ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo);
}