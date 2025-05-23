using POrpg.Dungeon;

namespace POrpg.Enemies.Behaviours;

public class AggressiveBehaviour : IBehaviour
{
    private Position? _lastSeenTarget;
    
    private bool CanSee(Dungeon.Dungeon dungeon, Position from, Position to)
    {
        // Bresenham's line algorithm
        var a = new Position(from.X, from.Y);
        var b = new Position(to.X, to.Y);
        var d = new Position(Math.Abs(b.X - a.X), -Math.Abs(b.Y - a.Y));
        var s = new Position(a.X < b.X ? 1 : -1, a.Y < b.Y ? 1 : -1);
        var err = d.X + d.Y;
        while (true)
        {
            if (dungeon[a].BlocksVision) return false;
            if (a == b) break;
            var e2 = 2 * err;
            if (e2 >= d.Y)
            {
                err += d.Y;
                a.X += s.X;
            }

            if (e2 <= d.X)
            {
                err += d.X;
                a.Y += s.Y;
            }
        }

        return true;
    }

    public Decision ComputeAction(Position position, Dungeon.Dungeon dungeon)
    {
        var target = dungeon.Players.Values
            .Where(p => CanSee(dungeon, position, p.Position))
            .MinBy(p => position.Distance(p.Position))
            ?.Position;
        if (target == null)
        {
            if (_lastSeenTarget == null)
                return new StayDecision();
            target = _lastSeenTarget;
        }

        _lastSeenTarget = target;

        var dx = target.Value.X - position.X;
        var dy = target.Value.Y - position.Y;

        if (Math.Abs(dx) > Math.Abs(dy))
            return new MoveDecision((dx > 0 ? 1 : -1, 0));
        return new MoveDecision((0, dy > 0 ? 1 : -1));
    }
}