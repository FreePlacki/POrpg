using POrpg.Commands;

namespace POrpg.InputHandlers;

public class CycleItemsHandler : InputHandler
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

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint(",.", "Cycle items", UiLocation.StandingOn);
        
        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}