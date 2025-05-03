using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public class QuitCommand : ICommand
{
    private readonly ConsoleView _view;

    public QuitCommand(ConsoleView view)
    {
        _view = view;
    }
    public void Execute()
    {
        _view.ShouldQuit = true;
    }
}