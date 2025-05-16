using POrpg.Commands;

namespace POrpg.InputHandlers;

public class InventoryInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.Q when keyInfo.Modifiers == ConsoleModifiers.None => new DropItemCommand(),
            ConsoleKey.Q when keyInfo.Modifiers == ConsoleModifiers.Shift => new DropAllItemsCommand(),
            _ => NextHandler!.HandleInput(keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("Q", "Drop", UiLocation.Inventory | UiLocation.Backpack);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}