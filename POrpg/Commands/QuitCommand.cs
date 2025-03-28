namespace POrpg.Commands;

public class QuitCommand : ICommand
{
    private Dungeon.Dungeon _dungeon;

    public QuitCommand(Dungeon.Dungeon dungeon)
    {
        _dungeon = dungeon;
    }
    public void Execute()
    {
        _dungeon.ShouldQuit = true;
    }
}