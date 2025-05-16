using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public class ChooseAttackCommand : ICommand
{
    private bool _cancel;

    public ChooseAttackCommand(bool cancel = false)
    {
        _cancel = cancel;
    }

    public bool AdvancesTurn => false;

    public void Execute(ConsoleView view)
    {
        view.IsChoosingAttack = !_cancel;
    }
}