using POrpg.ConsoleUtils;
using POrpg.Dungeon;

namespace POrpg.Enemies.Decisions;

public class AttackDecision : Decision
{
    private readonly Position _position;
    private readonly int _playerId;

    public AttackDecision(Position enemyPosition, int playerId)
    {
        _position = enemyPosition;
        _playerId = playerId;
    }

    public override void Execute(Dungeon.Dungeon dungeon)
    {
        var player = dungeon.Players[_playerId];
        var enemy = dungeon[_position].Enemy!;
        var damage = player.DealDamage(enemy.Damage, 0);
        ConsoleHelper.GetInstance().AddNotification($"{enemy.Name} attacks you for {damage}");
    }
}