using POrpg.Dungeon;

namespace POrpg.Enemies.Decisions;

public class MoveDecision : Decision
{
    private readonly Position _position;
    private readonly Position _direction;

    public MoveDecision(Position position, Position direction)
    {
        _position = position;
        _direction = direction;
    }

    public override void Execute(Dungeon.Dungeon dungeon)
    {
        var newPos = _position + _direction;
        if (!dungeon.CanMoveTo(newPos)) return;

        dungeon[newPos].Enemy = dungeon[_position].Enemy;
        dungeon[_position].Enemy = null;
    }
}