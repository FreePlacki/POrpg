using POrpg.Commands;
using POrpg.ConsoleHelpers;
using POrpg.Inventory;

namespace POrpg.InputHandlers;

public class InventoryInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleView view, Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.Q when keyInfo.Modifiers == ConsoleModifiers.None => new DropItemCommand(view, dungeon),
            ConsoleKey.Q when keyInfo.Modifiers == ConsoleModifiers.Shift => new DropAllItemsCommand(view, dungeon),
            _ => NextHandler!.HandleInput(view, dungeon, keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("Q", "Drop", UiLocation.Inventory | UiLocation.Backpack);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}