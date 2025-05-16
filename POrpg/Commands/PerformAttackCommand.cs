using POrpg.ConsoleHelpers;
using POrpg.Items.Weapons;

namespace POrpg.Commands;

public class PerformAttackCommand : ICommand
{
    private readonly IAttackVisitor _visitor;

    public PerformAttackCommand(IAttackVisitor visitor)
    {
        _visitor = visitor;
    }

    public bool AdvancesTurn => true;

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        dungeon.PerformAttack(_visitor, playerId);
    }

    public void Execute(ConsoleView view)
    {
        view.IsChoosingAttack = false;
    }
}