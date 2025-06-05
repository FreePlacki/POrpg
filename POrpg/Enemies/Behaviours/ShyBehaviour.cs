using POrpg.Dungeon;
using POrpg.Enemies.Decisions;

namespace POrpg.Enemies.Behaviours;

public class ShyBehaviour : IBehaviour
{
    private Position? _lastSeenTarget;

    public Decision ComputeAction(Position position, Dungeon.Dungeon dungeon)
    {
        var target = dungeon.Players.Values
            .Where(p => dungeon.CanSee(position, p.Position))
            .MinBy(p => position.Distance(p.Position))
            ?.Position;
        if (target == null)
        {
            if (_lastSeenTarget == null)
                return new StayDecision();
            target = _lastSeenTarget;
        }

        if (_lastSeenTarget != null && !dungeon.CanSee(position, _lastSeenTarget.Value))
        {
            _lastSeenTarget = null;
            if (target == null)
                return new StayDecision();
        }

        _lastSeenTarget = target;

        return new MoveDecision(position, target.Value, moveAway: true);
    }
}