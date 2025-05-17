using POrpg.ConsoleUtils;
using POrpg.Items.Weapons;

namespace POrpg.Commands;

public class PerformAttackCommand : ICommand
{
    public IAttackVisitor Visitor { get; }

    public PerformAttackCommand(IAttackVisitor visitor)
    {
        Visitor = visitor;
    }

    public bool AdvancesTurn => true;

    public void Execute(Dungeon.Dungeon dungeon, int playerId)
    {
        dungeon.PerformAttack(Visitor, playerId);
    }

    public void Execute(ConsoleView view)
    {
        view.IsChoosingAttack = false;
    }
}