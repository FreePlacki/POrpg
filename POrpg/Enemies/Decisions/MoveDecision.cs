using POrpg.Dungeon;

namespace POrpg.Enemies.Decisions;

public class MoveDecision : Decision
{
    public readonly Position Position;
    public readonly Position Target;

    public MoveDecision(Position position, Position target)
    {
        Position = position;
        Target = target;
    }

    public override void Execute(Dungeon.Dungeon dungeon)
    {
        var directions = new List<Position> { (0, 1), (1, 0), (0, -1), (-1, 0) };
        var newPos = Position;
        foreach (var dir in directions)
        {
            var pos = Position + dir;
            if (!dungeon.CanMoveTo(pos)) continue;
            if (pos.Distance(Target) < Position.Distance(Target))
                newPos = pos;
        }

        if (newPos == Position) return;

        dungeon[newPos].Enemy = dungeon[Position].Enemy;
        dungeon[Position].Enemy = null;
    }
}