using POrpg.Commands;
using POrpg.Items.Weapons;

namespace POrpg.InputHandlers;

public class AttackInputHandler : InputHandler
{
    private readonly Attributes _playerAttributes;

    public AttackInputHandler(Attributes playerAttributes)
    {
        _playerAttributes = playerAttributes;
    }

    public override ICommand HandleInput(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.N => new PerformAttackCommand(new NormalAttackVisitor(_playerAttributes)),
            ConsoleKey.S => new PerformAttackCommand(new StealthAttackVisitor(_playerAttributes)),
            ConsoleKey.M => new PerformAttackCommand(new MagicAttackVisitor(_playerAttributes)),
            ConsoleKey.C => new ChooseAttackCommand(cancel: true),
            _ => NextHandler!.HandleInput(keyInfo)
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