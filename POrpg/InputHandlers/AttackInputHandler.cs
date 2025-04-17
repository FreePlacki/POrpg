using POrpg.Commands;

namespace POrpg.InputHandlers;

public class AttackInputHandler : InputHandler
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.X => new MessageCommand(""),
            _ => NextHandler!.HandleInput(dungeon, keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("X", "Attack", UiLocation.Inventory);
        
        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}