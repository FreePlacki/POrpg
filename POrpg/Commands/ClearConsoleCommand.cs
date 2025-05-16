using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public class ClearConsoleCommand : ICommand
{
    public void Execute(ConsoleView _)
    {
        Console.Clear();
    }
}