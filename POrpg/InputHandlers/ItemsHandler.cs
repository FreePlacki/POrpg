using POrpg.Commands;

namespace POrpg.InputHandlers;

public class ItemsHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.E => new PickUpItemCommand(),
            _ => NextHandler!.HandleInput(keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("E", "Pick up", UiLocation.StandingOn);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}