using POrpg.Commands;
using POrpg.ConsoleHelpers;

namespace POrpg.InputHandlers;

public abstract class InputHandler
{
    protected InputHandler? NextHandler;

    public void SetNext(InputHandler? handler) => NextHandler = handler;

    public abstract ICommand HandleInput(ConsoleView view, Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo);
    
    public abstract IEnumerable<InputHint> GetHints();
}