using POrpg.ConsoleUtils;

namespace POrpg.Commands;

public class QuitCommand : ICommand
{
    public void Execute(ConsoleView view)
    {
        view.ShouldQuit = true;
    }
}