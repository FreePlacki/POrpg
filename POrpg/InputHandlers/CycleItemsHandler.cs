using POrpg.Commands;
using POrpg.ConsoleHelpers;

namespace POrpg.InputHandlers;

public class CycleItemsHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleView view, Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.OemPeriod => new CycleItemsCommand(view, dungeon),
            ConsoleKey.OemComma => new CycleItemsCommand(view, dungeon, reverse: true),
            _ => NextHandler!.HandleInput(view, dungeon, keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint(",.", "Cycle items", UiLocation.StandingOn);
        
        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}