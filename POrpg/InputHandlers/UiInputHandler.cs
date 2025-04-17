using POrpg.Commands;
using POrpg.Inventory;

namespace POrpg.InputHandlers;

public class UiInputHandler : InputHandler
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.C => new ClearConsoleCommand(),
            ConsoleKey.X => new QuitCommand(dungeon),
            _ when keyInfo.KeyChar == '?' => new ShowInstructionsCommand(),
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
    
    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("C", "Clear", UiLocation.Bottom);
        yield return new InputHint("X", "Quit", UiLocation.Bottom);
        yield return new InputHint("?", "Help", UiLocation.Bottom);
        
        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}