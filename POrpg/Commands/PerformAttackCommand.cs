using POrpg.ConsoleHelpers;
using POrpg.Items.Weapons;

namespace POrpg.Commands;

public class PerformAttackCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly ConsoleView _view;
    private readonly IAttackVisitor _visitor;

    public PerformAttackCommand(ConsoleView view, Dungeon.Dungeon dungeon, IAttackVisitor visitor)
    {
        _view    = view;
        _dungeon = dungeon;
        _visitor = visitor;
    }

    public bool AdvancesTurn => true;

    public void Execute()
    {
        _dungeon.PerformAttack(_visitor, _view.PlayerId);
        _view.IsChoosingAttack = false;
    }
}