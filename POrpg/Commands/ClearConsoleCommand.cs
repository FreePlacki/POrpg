namespace POrpg.Commands;

public class ClearConsoleCommand : ICommand
{
    public void Execute()
    {
        Console.Clear();
    }
}