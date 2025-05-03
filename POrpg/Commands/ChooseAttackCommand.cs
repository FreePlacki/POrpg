using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public class ChooseAttackCommand : ICommand
{
    private readonly ConsoleView _view;
    private bool _cancel;

    public ChooseAttackCommand(ConsoleView view, bool cancel = false)
    {
        _view = view;
        _cancel = cancel;
    }

    public bool AdvancesTurn => false;

    public void Execute()
    {
        _view.IsChoosingAttack = !_cancel;
    }
}