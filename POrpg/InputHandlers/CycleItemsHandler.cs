using POrpg.Commands;

namespace POrpg.InputHandlers;

public class CycleItemsHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.OemPeriod => new CycleItemsCommand(),
            ConsoleKey.OemComma => new CycleItemsCommand(reverse: true),
            _ => NextHandler!.HandleInput(keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint(",.", "Cycle items", UiLocation.StandingOn);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}