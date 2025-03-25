namespace POrpg.Commands;

public class MoveToBackpackCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;

    public MoveToBackpackCommand(Dungeon.Dungeon dungeon)
    {
        _dungeon = dungeon;
    }

    public bool IncreasesCounter => false;

    public void Execute()
    {
        _dungeon.TryMoveToBackpack();
    }
}