namespace POrpg.Commands;

public class MoveToBackpackCommand : ICommand
{
    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        dungeon.TryMoveToBackpack(playerId);
    }
}