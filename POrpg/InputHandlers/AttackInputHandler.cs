using POrpg.Combat;
using POrpg.Commands;

namespace POrpg.InputHandlers;

public class AttackInputHandler : InputHandler
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.N => new PerformAttackCommand(dungeon, new NormalAttackVisitor(dungeon.Player.Attributes)),
            ConsoleKey.S => new PerformAttackCommand(dungeon, new StealthAttackVisitor(dungeon.Player.Attributes)),
            ConsoleKey.M => new PerformAttackCommand(dungeon, new MagicAttackVisitor(dungeon.Player.Attributes)),
            ConsoleKey.C => new ChooseAttackCommand(dungeon, cancel: true),
            _ => NextHandler!.HandleInput(dungeon, keyInfo)
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