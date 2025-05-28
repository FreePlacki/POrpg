using POrpg.Dungeon;
using POrpg.Enemies.Decisions;

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

        if (position.Distance(target.Value) == 1)
        {
            var playerToAttack = dungeon.Players
                .Where(p => p.Value.Position == target.Value)
                .Select(p => p.Key).ToArray();
            if (playerToAttack.Length != 0)
                return new AttackDecision(position, playerToAttack.First());
            return new StayDecision();
        }

        var dx = target.Value.X - position.X;
        var dy = target.Value.Y - position.Y;

        if (Math.Abs(dx) > Math.Abs(dy))
            return new MoveDecision(position, (dx > 0 ? 1 : -1, 0));
        return new MoveDecision(position, (0, dy > 0 ? 1 : -1));
    }
}