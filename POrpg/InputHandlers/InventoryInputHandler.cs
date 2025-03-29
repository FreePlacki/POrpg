using POrpg.Commands;
using POrpg.Inventory;

namespace POrpg.InputHandlers;

public class InventoryInputHandler : InputHandler
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.E => new PickUpItemCommand(dungeon),
            ConsoleKey.Q when keyInfo.Modifiers == ConsoleModifiers.None => new DropItemCommand(dungeon),
            ConsoleKey.Q when keyInfo.Modifiers == ConsoleModifiers.Shift => new DropAllItemsCommand(dungeon),
            ConsoleKey.L => new SelectItemCommand(dungeon, new EquipmentSlot(EquipmentSlotType.LeftHand)),
            ConsoleKey.R => new SelectItemCommand(dungeon, new EquipmentSlot(EquipmentSlotType.RightHand)),
            ConsoleKey.B => new MoveToBackpackCommand(dungeon),
            ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.D3 or ConsoleKey.D4 or ConsoleKey.D5 or ConsoleKey.D6
                or ConsoleKey.D7 or ConsoleKey.D8 or ConsoleKey.D9
                when int.TryParse(keyInfo.KeyChar.ToString(), out var slot) =>
                new SelectItemCommand(dungeon, new BackpackSlot(slot - 1)),
            _ => NextHandler!.HandleInput(dungeon, keyInfo)
        };
    }
}