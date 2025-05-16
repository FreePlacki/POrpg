using POrpg.Commands;

namespace POrpg.InputHandlers;

public class ChooseAttackInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.X => new ChooseAttackCommand(),
            _ => NextHandler!.HandleInput(keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("X", "Attack", UiLocation.LookingAt);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}