using POrpg.Commands;
using POrpg.Inventory;

namespace POrpg.InputHandlers;

public class InventoryInputHandler : InputHandler
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.Q when keyInfo.Modifiers == ConsoleModifiers.None => new DropItemCommand(dungeon),
            ConsoleKey.Q when keyInfo.Modifiers == ConsoleModifiers.Shift => new DropAllItemsCommand(dungeon),
            _ => NextHandler!.HandleInput(dungeon, keyInfo)
        };
    }
    
    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("Q", "Drop", UiLocation.Inventory | UiLocation.Backpack);
        
        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}