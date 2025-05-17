using System.Text.Json.Serialization;
using POrpg.ConsoleUtils;
using POrpg.Items.Weapons;

namespace POrpg.Commands;

public class PerformAttackCommand : ICommand
{
    [JsonInclude] private readonly IAttackVisitor _visitor;

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