using POrpg.Commands;

namespace POrpg.InputHandlers;

public class ItemsHandler : InputHandler
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.E => new PickUpItemCommand(dungeon),
            _ => NextHandler!.HandleInput(dungeon, keyInfo)
        };
    }
    
    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("E", "Pick up", UiLocation.StandingOn);
        
        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}