using POrpg.Items.Weapons;

namespace POrpg.Commands;

public class PerformAttackCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly IAttackVisitor _visitor;

    public PerformAttackCommand(Dungeon.Dungeon dungeon, IAttackVisitor visitor)
    {
        _dungeon = dungeon;
        _visitor = visitor;
    }

    public bool AdvancesTurn => true;

    public void Execute()
    {
        _dungeon.PerformAttack(_visitor);
        _dungeon.IsChoosingAttack = false;
    }
}