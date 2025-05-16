using POrpg.Commands;

namespace POrpg.InputHandlers;

public class UsableItemInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.F => new UseItemCommand(),
            _ => NextHandler!.HandleInput(keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("F", "Use", UiLocation.Inventory | UiLocation.Backpack);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}