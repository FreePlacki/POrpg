using POrpg.Commands;
using POrpg.ConsoleHelpers;

namespace POrpg.InputHandlers;

public class ChooseAttackInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleView view, Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.X => new ChooseAttackCommand(view),
            _ => NextHandler!.HandleInput(view, dungeon, keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("X", "Attack", UiLocation.LookingAt);
        
        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}