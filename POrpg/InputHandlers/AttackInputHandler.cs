using POrpg.Commands;
using POrpg.ConsoleHelpers;
using POrpg.Items.Weapons;

namespace POrpg.InputHandlers;

public class AttackInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleView view, Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.N => new PerformAttackCommand(view, dungeon, new NormalAttackVisitor(view.Player.Attributes)),
            ConsoleKey.S => new PerformAttackCommand(view, dungeon, new StealthAttackVisitor(view.Player.Attributes)),
            ConsoleKey.M => new PerformAttackCommand(view, dungeon, new MagicAttackVisitor(view.Player.Attributes)),
            ConsoleKey.C => new ChooseAttackCommand(view, cancel: true),
            _ => NextHandler!.HandleInput(view, dungeon, keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("N", "Normal Attack", UiLocation.LookingAt);
        yield return new InputHint("S", "Stealth Attack", UiLocation.LookingAt);
        yield return new InputHint("M", "Magic Attack", UiLocation.LookingAt);
        yield return new InputHint("C", "Cancel", UiLocation.LookingAt);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}