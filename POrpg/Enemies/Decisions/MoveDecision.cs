using POrpg.Dungeon;

namespace POrpg.Enemies.Decisions;

public class MoveDecision : Decision
{
    private readonly Position _position;
    private readonly Position _target;
    private readonly bool _moveAway;

    public MoveDecision(Position position, Position target, bool moveAway = false)
    {
        _position = position;
        _target = target;
        _moveAway = moveAway;
    }

    public override void Execute(Dungeon.Dungeon dungeon)
    {
        var directions = new List<Position> { (0, 1), (1, 0), (0, -1), (-1, 0) };
        var newPos = _position;
        foreach (var dir in directions)
        {
            var pos = _position + dir;
            if (!dungeon.CanMoveTo(pos)) continue;
            if (_moveAway
                    ? pos.Distance(_target) > _position.Distance(_target)
                    : pos.Distance(_target) < _position.Distance(_target))
                newPos = pos;
        }

        if (newPos == _position) return;

        dungeon[newPos].Enemy = dungeon[_position].Enemy;
        dungeon[_position].Enemy = null;
    }
}