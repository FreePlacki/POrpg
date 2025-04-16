using POrpg.Commands;

namespace POrpg.InputHandlers;

public class AttackInputHandler : InputHandler
{
    public override ICommand HandleInput(Dungeon.Dungeon dungeon, ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            // TODO: composite commands can be made by constructing a different hadler chain
            // for example now we would want to switch to an attack handler chain
            // each InputHandler can then define it's description: Attack (X)
            // and whether it should be displayed under an item or at the bottom of the map
            // The dungeon should manage the input chain
            ConsoleKey.X => new MessageCommand(""),
            _ => NextHandler!.HandleInput(dungeon, keyInfo)
        };
    }
}