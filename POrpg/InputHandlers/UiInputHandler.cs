using POrpg.Commands;
using POrpg.Inventory;

namespace POrpg.InputHandlers;

public class UiInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.C => new ClearConsoleCommand(),
            _ when keyInfo.KeyChar == ']' => new QuitCommand(),
            _ when keyInfo.KeyChar == '?' => new ShowInstructionsCommand(),
            ConsoleKey.L => new SelectItemCommand(new EquipmentSlot(EquipmentSlotType.LeftHand)),
            ConsoleKey.R => new SelectItemCommand(new EquipmentSlot(EquipmentSlotType.RightHand)),
            ConsoleKey.B => new MoveToBackpackCommand(),
            ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.D3 or ConsoleKey.D4 or ConsoleKey.D5 or ConsoleKey.D6
                or ConsoleKey.D7 or ConsoleKey.D8 or ConsoleKey.D9
                when int.TryParse(keyInfo.KeyChar.ToString(), out var slot) =>
                new SelectItemCommand(new BackpackSlot(slot - 1)),
            _ => NextHandler!.HandleInput(keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("C", "Clear", UiLocation.Bottom);
        yield return new InputHint("]", "Quit", UiLocation.Bottom);
        yield return new InputHint("?", "Help", UiLocation.Bottom);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}