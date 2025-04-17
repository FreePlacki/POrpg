namespace POrpg.Commands;

public class ChooseAttackCommand : ICommand
{
    private readonly Dungeon.Dungeon _dungeon;
    private bool _cancel;

    public ChooseAttackCommand(Dungeon.Dungeon dungeon, bool cancel = false)
    {
        _dungeon = dungeon;
        _cancel = cancel;
    }

    public bool AdvancesTurn => false;

    public void Execute()
    {
        _dungeon.IsChoosingAttack = !_cancel;
    }
}