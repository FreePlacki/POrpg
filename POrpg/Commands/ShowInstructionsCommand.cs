using POrpg.ConsoleUtils;

namespace POrpg.Commands;

public class ShowInstructionsCommand : ICommand
{
    public void Execute(ConsoleView _)
    {
        ConsoleHelper.GetInstance().ShowInstructions();
    }
}