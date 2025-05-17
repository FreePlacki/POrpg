using POrpg.ConsoleUtils;

namespace POrpg.Commands;

public class ChooseAttackCommand : ICommand
{
    public bool Cancel { get; }

    public ChooseAttackCommand(bool cancel = false)
    {
        Cancel = cancel;
    }

    public bool AdvancesTurn => false;

    public void Execute(ConsoleView view)
    {
        view.IsChoosingAttack = !Cancel;
    }
}