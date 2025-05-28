using POrpg.Dungeon;

namespace POrpg.Enemies.Decisions;

public class MoveDecision : Decision
{
    public readonly Position Position;
    public readonly Position Direction;

    public MoveDecision(Position position, Position direction)
    {
        Position = position;
        Direction = direction;
    }

    public override void Execute(Dungeon.Dungeon dungeon)
    {
        var newPos = Position + Direction;
        if (!dungeon.CanMoveTo(newPos)) return;

        dungeon[newPos].Enemy = dungeon[Position].Enemy;
        dungeon[Position].Enemy = null;
    }
}