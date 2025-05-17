using System.Text.Json.Serialization;
using POrpg.ConsoleUtils;

namespace POrpg.Commands;

public class ChooseAttackCommand : ICommand
{
    [JsonInclude] private readonly bool _cancel;

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