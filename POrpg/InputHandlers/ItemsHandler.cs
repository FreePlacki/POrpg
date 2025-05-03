using POrpg.Commands;
using POrpg.ConsoleHelpers;

namespace POrpg.InputHandlers;

public class ItemsHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleView view, Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.E => new PickUpItemCommand(view, dungeon),
            _ => NextHandler!.HandleInput(view, dungeon, keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("E", "Pick up", UiLocation.StandingOn);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}